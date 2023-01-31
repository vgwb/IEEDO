using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Transition;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Ieedo
{
    public class UICardListScreen : UIScreen
    {
        public static bool FRONT_VIEW_ONLY = true;

        public static int VALIDATED_CARDS_PILLAR_INDEX = 0;
        public static int COMPLETED_CARDS_PILLAR_INDEX = 1;

        public UICardCollection CardsList;
        public GameObject FrontView;
        public RectTransform FrontViewPivot;
        public RectTransform FrontInteractionPivot;

        public UIButton CreateCardButton;
        private Coroutine createCardFlowCo;

        public UIOptionsListPopup optionsListPopup;

        public Image FrontObscurer;

        [Header("Card View")]
        public GameObject ViewMode;
        public UIButton CompleteCardButton;
        public UIButton EditCardButton;
        public UIButton UnCompleteCardButton_View;
        public Action OnCompletedCard;

        [Header("Review: Card Completed")]
        public GameObject CompletedMode;
        public UIButton ValidateCardButton;
        public UIButton UnCompleteCardButton_Review;
        public Action OnUncompleteCard;
        public Action OnValidateCard;

        [Header("Review: Card Validated")]
        public GameObject ValidatedMode;
        public UIButton UnValidateCardButton;

        [Header("Card Create & Edit")]
        public GameObject EditMode;
        public GameObject EditModeCardInteraction;
        public TMP_InputField DescriptionInputField;
        public TMP_InputField TitleInputField;
        public UIButton EditDifficultyButton;
        public UIButton EditDateButton;
        public UIButton EditSubCategoryButton;
        public UIButton EditTitleButton;
        public UIButton EditDescriptionButton;

        public UIButton DeleteCardButton;
        public UIButton CloseFrontViewButton;

        public UIText CardsCounter;

        public Action OnCreateClicked;

        [Header("Tutorial")]
        public UITextContent TutorialNoCards;

        public override ScreenID ID => ScreenID.CardList;
        public bool KeepPillars { get; set; }

        void Awake()
        {
            SetupButton(ValidateCardButton, () => StartCoroutine(ValidateCardCO(frontCardUI)));
            SetupButton(CompleteCardButton, () => StartCoroutine(CompleteCardCO(frontCardUI)));
            SetupButton(UnValidateCardButton, () => StartCoroutine(UnValidateCardCO(frontCardUI)));
            SetupButton(UnCompleteCardButton_View, () => StartCoroutine(UnCompleteCardCO(frontCardUI)));
            SetupButton(UnCompleteCardButton_Review, () => StartCoroutine(UnCompleteCardCO(frontCardUI)));
            SetupButton(EditCardButton, () => SetEditing(!canEdit));
            SetupButton(CreateCardButton, () =>
            {
                OnCreateClicked?.Invoke();
                createCardFlowCo = StartCoroutine(CreateCardFlowCO());
            });

            CardsList.ScrollRect.OnDragStart = () =>
            {
                if (!CardsList.ScrollRect.enabled)
                    return;
                SetButtonsVisible(false);
            };
            /*CardsList.ScrollRect.OnCardNotInFront = () => SetButtonsVisible(false);*/
            CardsList.ScrollRect.OnCardInFront = () =>
            {
                SetButtonsVisible(true);
                RefreshFrontViewMode();
            };
            //cardScroll.OnCardSelected = () => SetButtonsVisible(true);

            CardsList.OnCardClicked = _ =>
            {
                /* @note: this was used only in the old interaction view
                // Per-Card view mode when we are in pillars view (completed / validated cards)
                switch (uiCard.Data.Status)
                {
                    case CardStatus.Completed:
                        desiredFrontViewMode = FrontViewMode.Completed;
                        break;
                    case CardStatus.Validated:
                        desiredFrontViewMode = FrontViewMode.Validated;
                        break;
                }
                OpenFrontView(uiCard, desiredFrontViewMode);*/
            };

            SetupButton(CloseFrontViewButton, () =>
            {
                if (CurrentFrontViewMode == FrontViewMode.Create && createCardFlowCo != null)
                {
                    StartCoroutine(CreationAbortCO());
                    return;
                }

                if (optionsListPopup.isActiveAndEnabled)
                {
                    // We should actually close this (quit editing)
                    optionsListPopup.CloseImmediate();
                    return;
                }

                bool canCloseFront = !frontCardUI.Description.text.IsNullOrEmpty() && !frontCardUI.Title.text.IsNullOrEmpty();
                if (!canCloseFront)
                {
                    Debug.LogWarning("Cannot close front. Card is incomplete.");
                    return;
                }
                var cardUI = frontCardUI;

                StopEditing();
                CloseFrontView();
                cardUI.RefreshUI();
            });


        }

        #region Card Editing

        private bool canEdit = false;
        private void SetEditing(bool choice)
        {
            canEdit = choice;
            EditModeCardInteraction.SetActive(canEdit);
            if (canEdit)
            {
                EditModeCardInteraction.transform.SetParent(frontCardUI.transform);
                EditModeCardInteraction.transform.localPosition = Vector3.zero;
            }
            else
            {
                EditModeCardInteraction.transform.SetParent(transform);
            }
            EditCardButton.Text = canEdit ? "\uf14a" : "\uf044";

            CardsList.ScrollRect.enabled = !canEdit;

            if (CurrentFrontViewMode == FrontViewMode.Edit)
            {
                if (canEdit)
                {
                    CreateCardButton.Hide();
                    CompleteCardButton.Hide();
                    DeleteCardButton.Hide();
                }
                else
                {
                    CreateCardButton.Show();
                    CompleteCardButton.Show();
                    DeleteCardButton.Show();
                }
            }
        }

        #endregion

        private IEnumerator DeleteCardCO(bool isNewCard, UICard uiCard, bool withConfirmation = true)
        {
            if (withConfirmation)
            {
                var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
                Ref<int> selection = new Ref<int>();
                yield return questionScreen.ShowQuestionFlow(new LocalizedString("UI", "delete_card_confirmation_title"),
                    new LocalizedString("UI", "delete_card_confirmation_description")
                    , new[] {
                        new LocalizedString("UI","yes"),
                        new LocalizedString("UI","no")
                    }, selection);
                if (selection.Value == 1)
                    yield break;
            }

            StopEditing();

            yield return AnimateCardOut(uiCard, 0, -1);
            if (uiCard != null)
                Destroy(uiCard.gameObject);
            if (!FRONT_VIEW_ONLY)
                CloseFrontView();

            CardsList.RemoveCard(uiCard);

            Statics.Analytics.Card("delete", uiCard.Data);
            Statics.Cards.DeleteCard(uiCard.Data);
            Statics.Cards.DeleteCardDefinition(uiCard.Data.Definition);

            CheckEmptyHand();
        }

        private void CheckEmptyHand()
        {
            CardsList.SortListAgain();
            if (CardsList.HeldCards.Count == 0)
            {
                CloseFrontView();
                CreateCardButton.Show();
            }
        }

        private IEnumerator CompleteCardCO(UICard uiCard)
        {
            /*
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            Ref<int> selection = new Ref<int>();
            yield return questionScreen.ShowQuestionFlow(new LocalizedString("UI","complete_card_confirmation_title"),
                new LocalizedString("UI","complete_card_confirmation_description")
                , new [] {
                    new LocalizedString("UI","yes"),
                    new LocalizedString("UI","no")
                }, selection);
            if (selection.Value == 1)
                yield break;
                */

            frontCardUI.Data.CompletionTimestamp = Timestamp.Now;
            frontCardUI.Data.Status = CardStatus.Completed;
            Statics.Data.SaveProfile();

            yield return AnimateCardStatusChange(uiCard, Statics.App.ApplicationConfig.PointsCardCompleted);

            yield return AnimateCardOut(uiCard, +1);
            if (uiCard != null)
                Destroy(uiCard.gameObject);
            if (!FRONT_VIEW_ONLY)
                CloseFrontView();

            CardsList.RemoveCard(frontCardUI);

            Statics.Screens.GoTo(ScreenID.Pillars);

            Statics.Analytics.Card("complete", uiCard.Data);
            OnCompletedCard?.Invoke();
        }

        private IEnumerator AnimateCardStatusChange(UICard uiCard, int points)
        {
            uiCard.transform.localScaleTransition(new Vector3(1, 1, 1) * 1.2f, 0.25f, LeanEase.Elastic);
            uiCard.StampGO.GetComponent<Animation>().Play("stamp_exit");
            yield return new WaitForSeconds(0.25f);
            Statics.Points.AddPoints(points);
            uiCard.RefreshUI();
            uiCard.StampGO.GetComponent<Animation>().Play("stamp_enter");
            uiCard.transform.localScaleTransition(new Vector3(1, 1, 1) * 1f, 0.25f, LeanEase.Elastic);
            yield return new WaitForSeconds(0.25f);
        }

        private IEnumerator AnimateCardOut(UICard uiCard, int direction, int yDirection = 1)
        {
            var period = 0.5f;
            uiCard.transform.localPositionTransition(new Vector3(direction * 300, yDirection * 600, -150), period, LeanEase.Accelerate);
            uiCard.transform.localEulerAnglesTransform(new Vector3(0, direction * 20, direction * 20), period, LeanEase.Accelerate);
            yield return new WaitForSeconds(period);
        }

        private IEnumerator UnCompleteCardCO(UICard uiCard)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            Ref<int> selection = new Ref<int>();
            yield return questionScreen.ShowQuestionFlow(new LocalizedString("UI", "uncomplete_card_confirmation_title"),
                new LocalizedString("UI", "uncomplete_card_confirmation_description")
                , new[] {
                    new LocalizedString("UI","yes"),
                    new LocalizedString("UI","no")
                }, selection);
            if (selection.Value == 1)
                yield break;

            frontCardUI.Data.CompletionTimestamp = Timestamp.None;
            frontCardUI.Data.Status = CardStatus.Todo;
            Statics.Data.SaveProfile();

            yield return AnimateCardStatusChange(uiCard, -Statics.App.ApplicationConfig.PointsCardCompleted);

            yield return AnimateCardOut(uiCard, -1);

            CardsList.RemoveCard(frontCardUI);

            if (uiCard != null)
                Destroy(uiCard.gameObject);
            if (!FRONT_VIEW_ONLY)
                CloseFrontView();

            // Remove the card from the current pillar
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            uiPillarsScreen.PillarsManager.CurrentFocusedPillar.RemoveSingleCard(uiCard.Data);

            Statics.Analytics.Card("uncomplete", uiCard.Data);
            OnUncompleteCard?.Invoke();
        }

        private IEnumerator ValidateCardCO(UICard uiCard)
        {
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;

            frontCardUI.Data.ValidationTimestamp = Timestamp.Now;
            frontCardUI.Data.Status = CardStatus.Validated;
            Statics.Data.SaveProfile();

            yield return AnimateCardStatusChange(uiCard, Statics.App.ApplicationConfig.PointsCardValidated);

            if (uiPillarsScreen.ViewMode == PillarsViewMode.Review)
            {
                yield return AnimateCardOut(uiCard, 0);
                if (uiCard != null)
                    Destroy(uiCard.gameObject);
                if (!FRONT_VIEW_ONLY)
                    CloseFrontView();

                // Add the new card
                uiPillarsScreen.PillarsManager.PillarViews[COMPLETED_CARDS_PILLAR_INDEX].RemoveSingleCard(uiCard.Data);
                uiPillarsScreen.PillarsManager.PillarViews[VALIDATED_CARDS_PILLAR_INDEX].AddSingleCard(uiCard.Data);
            }
            else
            {
                // Must refresh buttons
                SwitchToFrontViewMode(FrontViewMode.Validated);
            }

            if (uiPillarsScreen.ViewMode == PillarsViewMode.Review)
            {
                CardsList.RemoveCard(frontCardUI);
            }

            Statics.Analytics.Card("validate", uiCard.Data);
            OnValidateCard?.Invoke();
        }

        private IEnumerator UnValidateCardCO(UICard uiCard)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            Ref<int> selection = new Ref<int>();
            yield return questionScreen.ShowQuestionFlow(new LocalizedString("UI", "unvalidate_card_confirmation_title"),
                new LocalizedString("UI", "unvalidate_card_confirmation_description")
                , new[] {
                    new LocalizedString("UI","yes"),
                    new LocalizedString("UI","no")
                }, selection);
            if (selection.Value == 1)
                yield break;

            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;

            frontCardUI.Data.ValidationTimestamp = Timestamp.None;
            frontCardUI.Data.Status = CardStatus.Completed;
            Statics.Data.SaveProfile();

            yield return AnimateCardStatusChange(uiCard, -Statics.App.ApplicationConfig.PointsCardValidated);

            if (uiPillarsScreen.ViewMode == PillarsViewMode.Review)
            {
                yield return AnimateCardOut(uiCard, 0);
                if (uiCard != null)
                    Destroy(uiCard.gameObject);
                if (!FRONT_VIEW_ONLY)
                    CloseFrontView();

                // Add the new card
                uiPillarsScreen.PillarsManager.PillarViews[VALIDATED_CARDS_PILLAR_INDEX].RemoveSingleCard(uiCard.Data);
                uiPillarsScreen.PillarsManager.PillarViews[COMPLETED_CARDS_PILLAR_INDEX].AddSingleCard(uiCard.Data);
            }
            else
            {
                // Must refresh buttons
                SwitchToFrontViewMode(FrontViewMode.Completed);
            }

            if (uiPillarsScreen.ViewMode == PillarsViewMode.Review)
            {
                CardsList.RemoveCard(frontCardUI);
            }

            Statics.Analytics.Card("unvalidate", uiCard.Data);
        }

        protected override IEnumerator OnOpen()
        {
            var cardScroll = CardsList.GetComponentInChildren<SnappingScrollRect>();
            //if (CurrentListViewMode == ListViewMode.ToDo) cardScroll.ForceToPos(0f);
            yield return base.OnOpen();
            if (CardsList.HeldCards.Count > 0)
                yield return cardScroll.ForceGoToCard(CardsList.HeldCards[0], immediate: true);
        }

        protected override IEnumerator OnClose()
        {
            CloseFrontView();
            yield return base.OnClose();
        }

        public void Update()
        {
            TutorialNoCards.gameObject.SetActive(CurrentListViewMode == ListViewMode.ToDo && CardsList.HeldCards.Count == 0 && !FrontView.gameObject.activeSelf);
        }

        private UICard frontCardUI;
        private Transform prevFrontCardParent;

        public void ForceFrontCard(UICard cardUI)
        {
            this.frontCardUI = cardUI;
            //Debug.LogError("FRONT CARD IS NOW " + cardUI);
            CardsCounter.Text = $"{CardsList.HeldCards.IndexOf(cardUI) + 1}/{CardsList.HeldCards.Count}";
        }

        public static int SortByExpirationDate(CardData c1, CardData c2)
        {
            return c1.ExpirationTimestamp.Date.CompareTo(c2.ExpirationTimestamp.Date);
        }

        private FrontViewMode desiredFrontViewMode;
        public void LoadCards(List<CardData> cards, Func<CardData, CardData, int> sort, ListViewMode listViewMode, FrontViewMode frontViewMode)
        {
            FrontView.gameObject.SetActive(false);

            CurrentListViewMode = listViewMode;
            desiredFrontViewMode = frontViewMode;

            // Setup for this view
            //var rt = CardsList.GetComponent<RectTransform>();
            //var layout = CardsList.GetComponentInChildren<LayoutGroup>(true);
            switch (CurrentListViewMode)
            {
                case ListViewMode.ToDo:
                    CreateCardButton.gameObject.SetActive(true);
                    DefaultOutEnterPosition = new Vector3(0, -2500, 0);
                    DefaultOutExitPosition = new Vector3(0, -2500, 0);
                    //rt.anchorMin = new Vector2(0, 0);
                    //rt.anchorMax = new Vector2(0, 1);
                    //rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
                    //rt.localPosition = new Vector3(-220, 0, 0);
                    //layout.padding.bottom = 300;
                    break;
                case ListViewMode.Pillars:
                    CreateCardButton.gameObject.SetActive(false);
                    DefaultOutEnterPosition = new Vector3(0, 2500, 0);
                    DefaultOutExitPosition = new Vector3(0, 2500, 0);
                    //rt.anchorMin = new Vector2(0, 0);
                    //rt.anchorMax = new Vector2(1, 1);
                    //rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
                    //CardsList.transform.localPosition = new Vector3(153.734985f, 0, 0);
                    //layout.padding.bottom = 600;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var collection = new CardDataCollection();
            collection.AddRange(cards);
            CardsList.AssignList(collection);
            CardsList.SortList(sort);
            CardsList.AnimateEntrance(CurrentListViewMode);
        }

        public void LoadToDoCards()
        {
            var cards = Statics.Data.Profile.Cards.Where(x => x.Status == CardStatus.Todo).ToList();
            LoadCards(cards, SortByExpirationDate, ListViewMode.ToDo, FrontViewMode.Edit);
        }

        public enum ListViewMode
        {
            ToDo,
            Pillars,
        }

        public enum FrontViewMode
        {
            None,
            View,
            Create,
            Edit,
            Completed,
            Validated
        }

        public ListViewMode CurrentListViewMode;
        public FrontViewMode CurrentFrontViewMode;

        public void OpenFrontView(UICard uiCard, FrontViewMode viewMode)
        {
            //Debug.LogError("OPENING FRONT VIEW " + viewMode);

            if (CurrentFrontViewMode == FrontViewMode.None)
            {
                FrontObscurer.colorTransition(new Color(FrontObscurer.color.r, FrontObscurer.color.g, FrontObscurer.color.b, 0.5f), 0.25f);
            }

            if (!FRONT_VIEW_ONLY)
            {
                prevFrontCardParent = uiCard.transform.parent;
                var uiCardRt = uiCard.GetComponent<RectTransform>();
                uiCardRt.SetParent(FrontViewPivot, true);
                uiCardRt.anchorMin = new Vector2(0.5f, 0.5f);
                uiCardRt.anchorMax = new Vector2(0.5f, 0.5f);
                uiCardRt.SetAsFirstSibling();
                uiCard.AnimateToParent();

                EditModeCardInteraction.transform.SetParent(uiCardRt);
                EditModeCardInteraction.transform.localScale = Vector3.one;
                EditModeCardInteraction.transform.localRotation = Quaternion.identity;
                EditModeCardInteraction.transform.localPosition = Vector3.zero;
            }

            FrontView.gameObject.SetActive(true);
            ForceFrontCard(uiCard);

            SwitchToFrontViewMode(viewMode);
        }

        public void RefreshFrontViewMode()
        {
            // Stop creating, as we are moving to another card
            if (CurrentFrontViewMode == FrontViewMode.Create)
            {
                CurrentFrontViewMode = FrontViewMode.Edit;
            }
            SwitchToFrontViewMode(CurrentFrontViewMode);
        }

        private void SwitchToFrontViewMode(FrontViewMode viewMode)
        {
            bool isSameMode = viewMode == CurrentFrontViewMode;
            if (isSameMode)
                return;

            SoundManager.I.PlaySfx(SfxEnum.open);
            CurrentFrontViewMode = viewMode;
            switch (viewMode)
            {
                case FrontViewMode.Edit:
                    //EditModeCardInteraction.SetActive(true);
                    EditMode.SetActive(true);
                    ViewMode.SetActive(false);
                    CompletedMode.SetActive(false);
                    ValidatedMode.SetActive(false);
                    StartEditing(false);

                    AnimateShowButton(CreateCardButton);
                    AnimateShowButton(DeleteCardButton);
                    CompleteCardButton.gameObject.SetActive(true);
                    AnimateShowButton(CompleteCardButton);
                    break;
                case FrontViewMode.Create:
                    //EditModeCardInteraction.SetActive(true);
                    EditMode.SetActive(true);
                    ViewMode.SetActive(false);
                    CompletedMode.SetActive(false);
                    ValidatedMode.SetActive(false);
                    StartEditing(true);

                    CreateCardButton.transform.localScale = Vector3.zero;
                    CompleteCardButton.gameObject.SetActive(false);
                    break;
                case FrontViewMode.View:
                    //EditModeCardInteraction.SetActive(false);
                    EditMode.SetActive(false);
                    ViewMode.SetActive(true);
                    CompletedMode.SetActive(false);
                    ValidatedMode.SetActive(false);

                    AnimateShowButton(UnCompleteCardButton_View);
                    break;
                case FrontViewMode.Completed:
                    //EditModeCardInteraction.SetActive(false);
                    EditMode.SetActive(false);
                    ViewMode.SetActive(false);
                    CompletedMode.SetActive(true);
                    ValidatedMode.SetActive(false);

                    AnimateShowButton(UnCompleteCardButton_Review);

                    bool canValidate = Statics.Mode.SessionMode == SessionMode.Session;
                    if (canValidate)
                    {
                        AnimateShowButton(ValidateCardButton);
                    }
                    else
                    {
                        ValidateCardButton.transform.localScale = Vector3.zero;
                    }
                    break;
                case FrontViewMode.Validated:
                    //EditModeCardInteraction.SetActive(false);
                    EditMode.SetActive(false);
                    ViewMode.SetActive(false);
                    CompletedMode.SetActive(false);
                    ValidatedMode.SetActive(true);

                    AnimateShowButton(UnValidateCardButton);
                    break;
            }
        }

        private void SetButtonsVisible(bool choice)
        {
            FrontInteractionPivot.gameObject.SetActive(choice);
        }

        private void AnimateShowButton(UIButton btn)
        {
            btn.AnimateAppear();
        }

        public void CloseFrontView()
        {

            if (canEdit)
                SetEditing(false);

            //EditModeCardInteraction.transform.SetParent(null);
            FrontObscurer.colorTransition(new Color(FrontObscurer.color.r, FrontObscurer.color.g, FrontObscurer.color.b, 0f), 0.25f);
            FrontView.gameObject.SetActive(false);

            if (frontCardUI != null)
            {
                if (prevFrontCardParent != null)
                {
                    frontCardUI.transform.SetParent(prevFrontCardParent, true);
                    frontCardUI.AnimateToParent();
                }
                CardsList.SetupInListInteraction(frontCardUI);
            }
            CurrentFrontViewMode = FrontViewMode.None;

            if (CurrentListViewMode == ListViewMode.ToDo)
            {
                AnimateShowButton(CreateCardButton);
            }

            // Make sure that the card is added correctly
            if (!FRONT_VIEW_ONLY)
            {
                if (frontCardUI != null)
                {
                    CardsList.AddCard(frontCardUI.Data, frontCardUI);
                    if (CurrentListViewMode == ListViewMode.ToDo)
                    {
                        var cardIndex = CardsList.HeldCards.IndexOf(frontCardUI);
                        var slot = CardsList.HeldSlots[cardIndex];
                        slot.transform.localEulerAngles = new Vector3(0, 0f, -5f);
                    }
                    CardsList.SortListAgain();
                }
            }

            ForceFrontCard(null);

            prevFrontCardParent = null;
        }


        #region Card Creation & Editing

        public void SetSubEditMode(bool choice)
        {
            EditModeCardInteraction.SetActive(!choice && !isCreating);

            CloseFrontViewButton.gameObject.SetActive(!choice);
            SetEditButtonsEnabled(!choice);

            CardsList.ScrollRect.enabled = !choice && !isCreating && !canEdit;
        }
        public void SetEditButtonsEnabled(bool choice)
        {
            EditSubCategoryButton.enabled = choice;
            EditDifficultyButton.enabled = choice;
            EditDateButton.enabled = choice;
            EditTitleButton.enabled = choice;
            EditDescriptionButton.enabled = choice;
        }

        public IEnumerator CreationAbortCO()
        {
            var answer = new Ref<int>();
            yield return Statics.Screens.ShowQuestionFlow("UI/abort_creation_title", "UI/abort_creation_question", new[] { "UI/yes", "UI/no" }, answer);
            if (answer.Value == 0)
            {
                if (optionsListPopup.isActiveAndEnabled)
                {
                    optionsListPopup.CloseImmediate();
                }

                abortingCreation = true;
                if (createCardFlowCo != null)
                    StopCoroutine(createCardFlowCo);
                createCardFlowCo = null;
                SetEditButtonsEnabled(true);

                yield return DeleteCardCO(true, frontCardUI, withConfirmation: false);
                abortingCreation = false;
                isCreating = false;

                SetEditing(false);
                CreateCardButton.Show();

                var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
                uiTopScreen.SwitchMode(TopBarMode.MainSection);

                CheckEmptyHand();
            }
        }

        private bool abortingCreation;
        private bool isCreating;
        public IEnumerator CreateCardFlowCO()
        {
            isCreating = true;
            var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            uiTopScreen.SwitchMode(TopBarMode.Special_CardCreation);

            TitleInputField.text = "";
            DescriptionInputField.text = "";

            SetEditing(false);
            CreateCardButton.Hide();

            // Create and show the card
            var cardDef = Statics.Cards.GenerateCardDefinition(
                new CardDefinition
                {
                    Category = CategoryID.None,
                    SubCategory = 0,
                    Description = new LocString { DefaultText = "" },
                    Difficulty = 0,
                    Title = new LocString { DefaultText = "" },
                },
                isDefaultCard: AppManager.I.ApplicationConfig.SaveCardsAsDefault
            );

            // Create a new Data for this profile for that card
            var cardData = new CardData
            {
                DefID = cardDef.UID,
                CreationTimestamp = new Timestamp(DateTime.Now),
                ExpirationTimestamp = Timestamp.None
            };
            var cardUi = UICardManager.I.CreateCardUI(cardData, FrontViewPivot);

            // We add it right away, now, so we can delete it from there
            CardsList.AddCard(cardData, cardUi);
            CardsList.SortListAgain();

            cardUi.transform.localPosition = new Vector3(0, 1000, 0);
            OpenFrontView(cardUi, FrontViewMode.Edit);
            cardUi.AnimateToParent();

            yield return CardsList.ScrollRect.ForceGoToCard(cardUi, immediate: true);
            // Make sure it is set as the correct front one
            ForceFrontCard(cardUi);

            var cardFlowIndex = 0;
            while (cardFlowIndex <= 6)
            {
                switch (cardFlowIndex)
                {
                    case 0:
                        yield return EditCategoryCO();
                        break;
                    case 1:
                        yield return EditSubCategoryCO(cardUi.Data.Definition.CategoryDefinition);
                        break;
                    case 2:
                        yield return EditTitleCO();
                        break;
                    case 3:
                        yield return EditDifficultyCO();
                        break;
                    case 4:
                        yield return EditDateCO(cardUi.Data.Definition);
                        break;
                    case 5:
                        yield return EditDescriptionCO();
                        break;
                }
                cardFlowIndex++;
            }

            frontCardUI.AnimateToParent();

            SetEditButtonsEnabled(true);
            SetEditing(false);  // @note: We go back to the basic view here

            Statics.Cards.AddCard(cardData);
            Statics.Analytics.Card("create", cardData);
            createCardFlowCo = null;

            uiTopScreen.SwitchMode(TopBarMode.MainSection);

            CardsList.SortListAgain();
            yield return CardsList.ScrollRect.ForceGoToCard(frontCardUI);
            isCreating = false;
        }

        private IEnumerator EditCategoryCO(bool autoReset = false)
        {
            SetSubEditMode(true);
            var catResult = new Ref<CategoryDefinition>();
            frontCardUI.transform.localPositionTransition(new Vector3(0, 0, 0), 0.25f);
            frontCardUI.transform.localScaleTransition(Vector3.one * 0.75f, 0.25f);
            var options = new List<OptionData>();
            var categories = Statics.Data.GetAll<CategoryDefinition>();
            foreach (var categoryDef in categories)
            {
                options.Add(
                    new OptionData
                    {
                        UseLocString = true,
                        Key = categoryDef.Title.Key,
                        Color = categoryDef.BaseColor,
                        ShowIconSquare = true
                    }
                );
            }
            CloseFrontViewButton.gameObject.SetActive(true);
            optionsListPopup.ShowOptions(new LocalizedString("UI", "creation_choose_category"), options);
            while (optionsListPopup.isActiveAndEnabled && !abortingCreation)
                yield return null;
            CloseFrontViewButton.gameObject.SetActive(false);
            catResult.Value = categories[optionsListPopup.LatestSelectedOption];
            var selectedCategory = catResult.Value;
            frontCardUI.Data.Definition.Category = selectedCategory.ID;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            frontCardUI.RefreshUI();
            if (autoReset)
                frontCardUI.AnimateToParent();
            SetSubEditMode(false);
        }

        private IEnumerator EditDifficultyCO(bool autoReset = false)
        {
            EditDifficultyButton.Shadow.enabled = true;
            frontCardUI.Difficulty.gameObject.SetActive(true);
            frontCardUI.Difficulty.SetValue(frontCardUI.Data.Definition.Difficulty);

            SetSubEditMode(true);
            var result = new Ref<int>();
            frontCardUI.transform.localPositionTransition(new Vector3(0, 50, 0), 0.25f);
            var options = new List<OptionData>();
            var possibleDifficulties = new[] { 1, 2, 3 };
            foreach (var possibleDifficulty in possibleDifficulties)
            {
                var optionData = new OptionData
                {
                    UseIconsString = true,
                    Text = "",
                    Color = Color.white,
                    ShowIconSquare = false
                };
                for (int i = 1; i <= 3; i++)
                {
                    if (i <= possibleDifficulty)
                        optionData.Text += "\uf005";
                    else
                        optionData.Text += "<color=#0003>\uf005</color>";
                }
                options.Add(optionData);
            }
            CloseFrontViewButton.gameObject.SetActive(true);
            optionsListPopup.ShowOptions(new LocalizedString("UI", "creation_choose_difficulty"), options);
            while (optionsListPopup.isActiveAndEnabled && !abortingCreation)
                yield return null;
            CloseFrontViewButton.gameObject.SetActive(false);
            result.Value = possibleDifficulties[optionsListPopup.LatestSelectedOption];
            var selection = result.Value;
            frontCardUI.Data.Definition.Difficulty = selection;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            frontCardUI.RefreshUI();
            if (autoReset)
                frontCardUI.AnimateToParent();
            SetSubEditMode(false);
            //EditDifficultyButton.Shadow.enabled = false;
        }

        private IEnumerator EditDateCO(CardDefinition cardDef, bool autoReset = false)
        {
            EditDateButton.Shadow.enabled = true;
            SetSubEditMode(true);
            var result = new Ref<int>();
            frontCardUI.transform.localPositionTransition(new Vector3(0, 200, 0), 0.25f);
            var options = new List<OptionData>();
            var possibleDays = new List<int>();
            int nMonths = 2;
            int nDaysPerMonth = 30;
            for (int i = 0; i < nMonths * nDaysPerMonth; i++)
                possibleDays.Add(i);
            foreach (var possibleDay in possibleDays)
            {
                var targetDate = DateTime.Now.AddDays(possibleDay);
                var color = cardDef.CategoryDefinition.LightColor;
                if (targetDate.DayOfWeek == DayOfWeek.Saturday)
                    color = cardDef.CategoryDefinition.DarkColor;
                if (targetDate.DayOfWeek == DayOfWeek.Sunday)
                    color = cardDef.CategoryDefinition.DarkColor;

                options.Add(
                    new OptionData
                    {
                        UseLocString = false,
                        Text = new Timestamp(targetDate).ToString(),
                        IconText = "\uf133",
                        Color = color,
                        ShowIconSquare = true
                    }
                );
            }
            CloseFrontViewButton.gameObject.SetActive(true);
            optionsListPopup.ShowOptions(new LocalizedString("UI", "creation_choose_date"), options);
            while (optionsListPopup.isActiveAndEnabled && !abortingCreation)
                yield return null;
            CloseFrontViewButton.gameObject.SetActive(false);
            result.Value = possibleDays[optionsListPopup.LatestSelectedOption];
            var selection = result.Value;
            frontCardUI.Data.ExpirationTimestamp = new Timestamp(DateTime.Now.AddDays(selection));
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            frontCardUI.RefreshUI();
            if (autoReset)
                frontCardUI.AnimateToParent();
            SetSubEditMode(false);
            //EditDateButton.Shadow.enabled = false;

            // Sort again if we are only editing
            if (!isCreating)
            {
                CardsList.SortListAgain();
                yield return CardsList.ScrollRect.ForceGoToCard(frontCardUI);
            }
        }

        private IEnumerator EditSubCategoryCO(CategoryDefinition categoryDef, bool autoReset = false)
        {
            isSubEditing = true;
            EditSubCategoryButton.Shadow.enabled = true;
            SetSubEditMode(true);
            var result = new Ref<SubCategoryDefinition>();
            frontCardUI.transform.localPositionTransition(new Vector3(0, -15, 0), 0.25f);
            var options = new List<OptionData>();
            var subCategories = categoryDef.SubCategories;
            foreach (var subCategoryDef in subCategories)
            {
                options.Add(
                    new OptionData
                    {
                        UseLocString = true,
                        Key = subCategoryDef.Title.Key,
                        Color = categoryDef.BaseColor,
                        IconText = subCategoryDef.Icon,
                        ShowIconSquare = true
                    }
                );
            }
            CloseFrontViewButton.gameObject.SetActive(true);
            optionsListPopup.ShowOptions(new LocalizedString("UI", "creation_choose_subcategory"), options);
            while (optionsListPopup.isActiveAndEnabled && !abortingCreation)
                yield return null;
            CloseFrontViewButton.gameObject.SetActive(false);
            result.Value = subCategories[optionsListPopup.LatestSelectedOption];
            var selection = result.Value;
            frontCardUI.Data.Definition.SubCategory = selection.ID;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            frontCardUI.RefreshUI();
            if (autoReset)
                frontCardUI.AnimateToParent();
            SetSubEditMode(false);
            //EditSubCategoryButton.Shadow.enabled = false;
            isSubEditing = false;
        }

        public IEnumerator EditTitleCO(bool autoReset = false)
        {
            TitleInputField.gameObject.SetActive(true);
            EditTitleButton.Shadow.enabled = true;
            SetSubEditMode(true);
            frontCardUI.transform.localPositionTransition(new Vector3(0, -130, 0), 0.25f);
            frontCardUI.transform.localScaleTransition(Vector3.one * 1.3f, 0.25f);
            //frontCardUI.transform.localEulerAnglesTransform(new Vector3(0, 10, 0), 0.25f);
            optionsListPopup.ShowOptions(new LocalizedString("UI", "creation_enter_title"), new List<OptionData>(), isTextEntry: true);
            yield return WaitForInputField(TitleInputField, frontCardUI.Title);
            optionsListPopup.CloseImmediate();
            frontCardUI.Data.Definition.Title.DefaultText = TitleInputField.text;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            if (autoReset)
                frontCardUI.AnimateToParent();
            SetSubEditMode(false);
            //EditTitleButton.Shadow.enabled = false;
            TitleInputField.gameObject.SetActive(false);
        }

        public IEnumerator EditDescriptionCO(bool autoReset = false)
        {
            DescriptionInputField.gameObject.SetActive(true);
            EditDescriptionButton.Shadow.enabled = true;
            SetSubEditMode(true);
            frontCardUI.transform.localPositionTransition(new Vector3(0, 120, 0), 0.25f);
            frontCardUI.transform.localScaleTransition(Vector3.one * 1.2f, 0.25f);
            //frontCardUI.transform.localEulerAnglesTransform(new Vector3(0, 10, 0), 0.25f);
            optionsListPopup.ShowOptions(new LocalizedString("UI", "creation_enter_description"), new List<OptionData>(), isTextEntry: true);
            yield return WaitForInputField(DescriptionInputField, frontCardUI.Description);
            optionsListPopup.CloseImmediate();
            frontCardUI.Data.Definition.Description.DefaultText = DescriptionInputField.text;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            if (autoReset)
                frontCardUI.AnimateToParent();
            SetSubEditMode(false);
            //EditDescriptionButton.Shadow.enabled = false;
            DescriptionInputField.gameObject.SetActive(false);
        }

        private IEnumerator WaitForInputField(TMP_InputField inputField, UIText uiText)
        {
            inputField.textComponent = uiText;
            inputField.text = uiText.text;
            inputField.placeholder.enabled = inputField.text.IsNullOrEmpty();
            do
            {
                // Wait for closing...
                /* TODO: MOVE HERE
                 while (CurrentFrontViewMode != FrontViewMode.None)
                 {
                     frontCardUI.Data.Definition.Title.DefaultText = TitleInputField.text;
                     frontCardUI.Data.Definition.Description.DefaultText = DescriptionInputField.text;
                     yield return null; // Wait for completion
                 }*/

                inputField.ActivateInputField();
                yield return null; // Must wait one frame
                while (inputField.isFocused)
                    yield return null;


            } while (inputField.text.IsNullOrEmpty() && !abortingCreation);
        }

        private bool isSubEditing;
        public bool IsSubEditing => isSubEditing;

        public void StartEditing(bool isNewCard)
        {
            //Debug.LogError("START EDITING (new card "+ isNewCard + ")");

            var cardUI = frontCardUI;
            if (cardUI != null)
            {
                DescriptionInputField.textComponent = cardUI.Description;
                DescriptionInputField.text = cardUI.Description.text;
                DescriptionInputField.placeholder.enabled = DescriptionInputField.text == "";

                TitleInputField.textComponent = cardUI.Title;
                TitleInputField.text = cardUI.Title.text;
                TitleInputField.placeholder.enabled = TitleInputField.text == "";
            }

            SetupButton(EditDifficultyButton, () => StartCoroutine(EditDifficultyCO(autoReset: true)));
            SetupButton(EditDateButton, () => StartCoroutine(EditDateCO(frontCardUI.Data.Definition, autoReset: true)));
            SetupButton(EditSubCategoryButton, () => StartCoroutine(EditSubCategoryCO(frontCardUI.Data.Definition.CategoryDefinition, autoReset: true)));
            SetupButton(EditTitleButton, () => StartCoroutine(EditTitleCO(autoReset: true)));
            SetupButton(EditDescriptionButton, () => StartCoroutine(EditDescriptionCO(autoReset: true)));
            SetupButton(DeleteCardButton, () => StartCoroutine(DeleteCardCO(isNewCard, frontCardUI)));

            // Make sure it is active before we can start the edit mode
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            StartCoroutine(EditModeCO());
        }

        private IEnumerator EditModeCO()
        {
            // Wait for closing...
            while (CurrentFrontViewMode != FrontViewMode.None)
            {
                frontCardUI.Data.Definition.Title.DefaultText = TitleInputField.text;
                frontCardUI.Data.Definition.Description.DefaultText = DescriptionInputField.text;

                yield return null; // Wait for completion
            }
        }

        public void StopEditing()
        {
            TitleInputField.textComponent = null;
            DescriptionInputField.textComponent = null;
        }

        #endregion

    }
}
