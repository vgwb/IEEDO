using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Transition;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UICardListScreen : UIScreen
    {
        public UICardCollection CardsList;
        public GameObject FrontView;
        public RectTransform FrontViewPivot;

        public UIButton CreateCardButton;

        public UIOptionsListPopup optionsListPopup;

        [Header("Card View")]
        public GameObject ViewMode;
        public UIButton CompleteCardButton;
        public UIButton EditCardButton;

        [Header("Card Review")]
        public GameObject CompletedMode;
        public UIButton ValidateCardButton;
        public UIButton UnCompleteCardButton;

        [Header("Card Validated")]
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
        public UIButton CreationBackButton;

        public override ScreenID ID => ScreenID.CardList;
        public bool KeepPillars { get; set; }

        void Awake()
        {
            CreationBackButton.gameObject.SetActive(false);

            SetupButton(ValidateCardButton, () => StartCoroutine(ValidateCardCO(frontCardUI)));
            SetupButton(CompleteCardButton, () => StartCoroutine(CompleteCardCO(frontCardUI)));
            SetupButton(UnValidateCardButton, () => StartCoroutine(UnValidateCardCO(frontCardUI)));
            SetupButton(UnCompleteCardButton, () => StartCoroutine(UnCompleteCardCO(frontCardUI)));

            SetupButton(EditCardButton, () => SwitchToFrontViewMode(FrontViewMode.Edit));

            SetupButton(CreateCardButton, () => StartCoroutine(CardCreationFlowCO()));

            CardsList.OnCardClicked = uiCard =>
            {
                OpenFrontView(uiCard, desiredFrontViewMode);
            };

            SetupButton(CloseFrontViewButton, () =>
            {
                bool canCloseFront = !frontCardUI.Description.text.IsNullOrEmpty() && !frontCardUI.Title.text.IsNullOrEmpty();
                if (!canCloseFront)
                {
                    Debug.LogWarning("Cannot close front. Card is incomplete.");
                    return;
                }
                var cardUI = frontCardUI;

                StopEditing();
                CloseFrontView();

                // TODO: if (isNewCard)
                    CardsList.AddCard(cardUI.Data, cardUI);
                // TODO: Re-Sort only if it was edited?
                //CardsList.SortList(SortByExpirationDate);
                cardUI.RefreshUI();
            });


        }

        private IEnumerator DeleteCardCO(bool isNewCard, UICard card)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            Ref<int> selection = new Ref<int>();
            yield return questionScreen.ShowQuestionFlow(new LocalizedString("UI","delete_card_confirmation_title"),
                new LocalizedString("UI","complete_card_confirmation_description")
                , new [] {
                    new LocalizedString("UI","yes"),
                    new LocalizedString("UI","no")
                }, selection);

            if (selection.Value == 1)
                yield break;

            if (!isNewCard) CardsList.RemoveCard(card);
            StopEditing();
            CloseFrontView();

            Statics.Cards.DeleteCard(card.Data);
            Statics.Cards.DeleteCardDefinition(card.Data.Definition);
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

            CardsList.RemoveCard(frontCardUI);

            frontCardUI.Data.CompletionTimestamp = Timestamp.Now;
            frontCardUI.Data.Status = CardValidationStatus.Completed;
            Statics.Data.SaveProfile();

            yield return AnimateCardOut(uiCard, +1);
            if (uiCard != null) Destroy(uiCard.gameObject);
            CloseFrontView();
            GoTo(ScreenID.Pillars);

            // Select pillars view
            var uiBottomScreen = Statics.Screens.Get(ScreenID.Bottom) as UIBottomScreen;
            uiBottomScreen.ToggleSelection(uiBottomScreen.btnPillars);

            Statics.Score.AddScore(20);
        }

        private IEnumerator AnimateCardOut(UICard uiCard, int direction)
        {
            var period = 0.5f;
            uiCard.transform.localPositionTransition(new Vector3(direction*300,600,-150), period, LeanEase.Accelerate);
            uiCard.transform.localEulerAnglesTransform(new Vector3(0,direction*20,direction*20), period, LeanEase.Accelerate);
            yield return new WaitForSeconds(period);
        }

        private IEnumerator UnCompleteCardCO(UICard uiCard)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            Ref<int> selection = new Ref<int>();
            yield return questionScreen.ShowQuestionFlow(new LocalizedString("UI","uncomplete_card_confirmation_title"),
                new LocalizedString("UI","uncomplete_card_confirmation_description")
                , new [] {
                    new LocalizedString("UI","yes"),
                    new LocalizedString("UI","no")
                }, selection);
            if (selection.Value == 1)
                yield break;

            CardsList.RemoveCard(frontCardUI);

            frontCardUI.Data.CompletionTimestamp = Timestamp.None;
            frontCardUI.Data.Status = CardValidationStatus.Todo;
            Statics.Data.SaveProfile();

            uiCard.transform.localPositionTransition(new Vector3(-300,600,-150), 0.5f, LeanEase.Accelerate);
            uiCard.transform.localEulerAnglesTransform(new Vector3(0,-20,-20), 0.5f, LeanEase.Accelerate);
            yield return new WaitForSeconds(0.5f);
            if (uiCard != null) Destroy(uiCard.gameObject);
            CloseFrontView();
            Statics.Score.AddScore(-20);
        }

        private IEnumerator ValidateCardCO(UICard uiCard)
        {
            CardsList.RemoveCard(frontCardUI);

            frontCardUI.Data.ValidationTimestamp = Timestamp.Now;
            frontCardUI.Data.Status = CardValidationStatus.Validated;
            Statics.Data.SaveProfile();

            yield return AnimateCardOut(uiCard, 1);
            if (uiCard != null) Destroy(uiCard.gameObject);
            CloseFrontView();

            // Just add the new card
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            int pillarIndex = 0;
            uiPillarsScreen.RefreshData(pillarIndex);
            // TODO: this can be done only with DATA Tho uiPillarsScreen.PillarsManager.RefreshPillarView(validatedCardsPillarIndex, true);

            Statics.Score.AddScore(50);
        }

        private IEnumerator UnValidateCardCO(UICard uiCard)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            Ref<int> selection = new Ref<int>();
            yield return questionScreen.ShowQuestionFlow(new LocalizedString("UI","unvalidate_card_confirmation_title"),
                new LocalizedString("UI","unvalidate_card_confirmation_description")
                , new [] {
                    new LocalizedString("UI","yes"),
                    new LocalizedString("UI","no")
                }, selection);
            if (selection.Value == 1)
                yield break;

            CardsList.RemoveCard(frontCardUI);

            frontCardUI.Data.ValidationTimestamp = Timestamp.None;
            frontCardUI.Data.Status = CardValidationStatus.Completed;
            Statics.Data.SaveProfile();

            yield return AnimateCardOut(uiCard, -1);
            if (uiCard != null) Destroy(uiCard.gameObject);
            CloseFrontView();

            // Just add the new card
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            int pillarIndex = 0;
            uiPillarsScreen.RefreshData(pillarIndex);
            // TODO: this can be done only with DATA Tho uiPillarsScreen.PillarsManager.RefreshPillarView(validatedCardsPillarIndex, true);

            Statics.Score.AddScore(-50);
        }

        protected override IEnumerator OnOpen()
        {
            FrontView.gameObject.SetActive(false);

            switch (CurrentListViewMode)
            {
                case ListViewMode.ToDo:
                    CreateCardButton.gameObject.SetActive(true);
                    break;
                default:
                    CreateCardButton.gameObject.SetActive(false);
                    break;
            }

            yield return base.OnOpen();
        }

        protected override IEnumerator OnClose()
        {
            CloseFrontView();
            yield return base.OnClose();
        }

        private UICard frontCardUI;
        private Transform prevFrontCardParent;

        public static int SortByExpirationDate(CardData c1, CardData c2)
        {
            return -c1.ExpirationTimestamp.Date.CompareTo(c2.ExpirationTimestamp.Date);
        }

        private FrontViewMode desiredFrontViewMode;
        public void LoadCards(List<CardData> cards, Func<CardData,CardData,int> sort, ListViewMode listViewMode, FrontViewMode frontViewMode)
        {
            CurrentListViewMode = listViewMode;
            desiredFrontViewMode = frontViewMode;

            // Setup for this view
            switch (CurrentListViewMode)
            {
                case ListViewMode.ToDo:
                    CardsList.transform.localPosition = new Vector3(-220,0,0);
                    DefaultOutEnterPosition = new Vector3(-2500, 0, 0);
                    DefaultOutExitPosition = new Vector3(-2500, 0, 0);
                    break;
                case ListViewMode.Pillars:
                    CardsList.transform.localPosition = new Vector3(153.734985f, 0, 0);
                    DefaultOutEnterPosition = new Vector3(0, 2500, 0);
                    DefaultOutExitPosition = new Vector3(0, 2500, 0);
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
            var cards = Statics.Data.Profile.Cards.Where(x => x.Status == CardValidationStatus.Todo).ToList();
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
        private FrontViewMode CurrentFrontViewMode;

        private void OpenFrontView(UICard uiCard, FrontViewMode viewMode)
        {
            if (CurrentFrontViewMode != FrontViewMode.None) return;

            prevFrontCardParent = uiCard.transform.parent;
            var uiCardRt = uiCard.GetComponent<RectTransform>();
            uiCardRt.SetParent(FrontViewPivot, true);
            uiCardRt.anchorMin = new Vector2(0.5f, 0.5f);
            uiCardRt.anchorMax = new Vector2(0.5f, 0.5f);
            uiCard.AnimateToParent();

            FrontView.gameObject.SetActive(true);
            frontCardUI = uiCard;

            SwitchToFrontViewMode(viewMode);
        }

        private void SwitchToFrontViewMode(FrontViewMode viewMode)
        {
            CurrentFrontViewMode = viewMode;
            switch (viewMode)
            {
                case FrontViewMode.Edit:
                    EditModeCardInteraction.SetActive(true);
                    EditMode.SetActive(true);
                    ViewMode.SetActive(false);
                    CompletedMode.SetActive(false);
                    ValidatedMode.SetActive(false);
                    StartEditing(false);
                    break;
                case FrontViewMode.Create:
                    EditModeCardInteraction.SetActive(true);
                    EditMode.SetActive(true);
                    ViewMode.SetActive(false);
                    CompletedMode.SetActive(false);
                    ValidatedMode.SetActive(false);
                    StartEditing(true);
                    break;
                case FrontViewMode.View:
                    EditModeCardInteraction.SetActive(false);
                    EditMode.SetActive(false);
                    ViewMode.SetActive(true);
                    CompletedMode.SetActive(false);
                    ValidatedMode.SetActive(false);
                    break;
                case FrontViewMode.Completed:
                    EditModeCardInteraction.SetActive(false);
                    EditMode.SetActive(false);
                    ViewMode.SetActive(false);
                    CompletedMode.SetActive(true);
                    ValidatedMode.SetActive(false);
                    break;
                case FrontViewMode.Validated:
                    EditModeCardInteraction.SetActive(false);
                    EditMode.SetActive(false);
                    ViewMode.SetActive(false);
                    CompletedMode.SetActive(false);
                    ValidatedMode.SetActive(true);
                    break;
            }
        }

        private void CloseFrontView()
        {
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
            frontCardUI = null;
            prevFrontCardParent = null;
            CurrentFrontViewMode = FrontViewMode.None;
        }


        #region Card Creation & Editing

        public void SetSubEditMode(bool choice)
        {
            CloseFrontViewButton.gameObject.SetActive(!choice);
            EditSubCategoryButton.gameObject.SetActive(!choice);
            EditDifficultyButton.gameObject.SetActive(!choice);
            EditDateButton.gameObject.SetActive(!choice);
            EditTitleButton.gameObject.SetActive(!choice);
            EditDescriptionButton.gameObject.SetActive(!choice);
        }

        public IEnumerator CardCreationFlowCO()
        {
            //CreationBackButton.gameObject.SetActive(true);

            // Create and show the card
            var cardDef = Statics.Cards.GenerateCardDefinition(
                new CardDefinition {
                    Category = CategoryID.None,
                    SubCategory = 0,
                    Description = new LocString { DefaultText = "" },
                    Difficulty = 0,
                    Title = new LocString { DefaultText = ""},
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
            var cardUi = UICardManager.I.AddCardUI(cardData, FrontViewPivot);
            cardUi.transform.localPosition = new Vector3(0, 1000, 0);
            OpenFrontView(cardUi, FrontViewMode.Create);
            cardUi.AnimateToParent();

            var cardFlowIndex = 0;
            while (cardFlowIndex <= 6)
            {
                switch (cardFlowIndex)
                {
                    case 0: yield return EditCategoryCO(); break;
                    case 1: yield return EditSubCategoryCO(cardUi.Data.Definition.CategoryDefinition); break;
                    case 2: yield return EditTitleCO(); break;
                    case 3: yield return EditDifficultyCO(); break;
                    case 4: yield return EditDateCO(); break;
                    case 5: yield return EditDescriptionCO(); break;
                }
                cardFlowIndex++;
            }

            frontCardUI.AnimateToParent();

            CreationBackButton.gameObject.SetActive(false);
            EditSubCategoryButton.gameObject.SetActive(true);
            EditDifficultyButton.gameObject.SetActive(true);
            EditDateButton.gameObject.SetActive(true);
            EditDescriptionButton.gameObject.SetActive(true);
            EditTitleButton.gameObject.SetActive(true);

            Statics.Cards.AddCard(cardData);
        }

        private IEnumerator EditCategoryCO(bool autoReset = false)
        {
            SetSubEditMode(true);
            var catResult = new Ref<CategoryDefinition>();
            frontCardUI.transform.localPositionTransition(new Vector3(0, 100, 0), 0.25f);
            var options = new List<OptionData>();
            var categories = Statics.Data.GetAll<CategoryDefinition>();
            foreach (var categoryDef in categories)
            {
                options.Add(
                    new OptionData
                    {
                        Text = categoryDef.Title.Text,
                        Color = categoryDef.Color
                    }
                );
            }
            optionsListPopup.ShowOptions(new LocalizedString("UI","creation_choose_category"), options);
            while (optionsListPopup.isActiveAndEnabled) yield return null;
            catResult.Value = categories[optionsListPopup.LatestSelectedOption];
            var selectedCategory = catResult.Value;
            frontCardUI.Data.Definition.Category = selectedCategory.ID;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            frontCardUI.RefreshUI();
            if (autoReset) frontCardUI.AnimateToParent();
            SetSubEditMode(false);
        }

        private IEnumerator EditDifficultyCO(bool autoReset = false)
        {
            SetSubEditMode(true);
            var result = new Ref<int>();
            frontCardUI.transform.localPositionTransition(new Vector3(0, 150, 0), 0.25f);
            var options = new List<OptionData>();
            var possibleDifficulties = new[] { 1, 2, 3 };
            foreach (var possibleDifficulty in possibleDifficulties)
            {
                var optionData = new OptionData {
                        Text = "",
                        Color = Color.white,
                    };
                for (int i = 1; i <= 3; i++) {
                    if (i <= possibleDifficulty)
                        optionData.Text += "\uf005";
                    else
                        optionData.Text += "<color=#0003>\uf005</color>";
                }
                options.Add(optionData);
            }
            optionsListPopup.ShowOptions(new LocalizedString("UI","creation_choose_difficulty"), options);
            while (optionsListPopup.isActiveAndEnabled) yield return null;
            result.Value = possibleDifficulties[optionsListPopup.LatestSelectedOption];
            var selection = result.Value;
            frontCardUI.Data.Definition.Difficulty = selection;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            frontCardUI.RefreshUI();
            if (autoReset) frontCardUI.AnimateToParent();
            SetSubEditMode(false);
        }

        private IEnumerator EditDateCO(bool autoReset = false)
        {
            SetSubEditMode(true);
            var result = new Ref<int>();
            frontCardUI.transform.localPositionTransition(new Vector3(0, 250, 0), 0.25f);
            var options = new List<OptionData>();
            var possibleDays = new List<int>();
            for (int i = 0; i < 3*7; i++) possibleDays.Add(i);
            foreach (var possibleDay in possibleDays)
            {
                var targetDate = DateTime.Now.AddDays(possibleDay);
                var color = Color.white;
                if (targetDate.DayOfWeek == DayOfWeek.Saturday) color = Color.red;
                if (targetDate.DayOfWeek == DayOfWeek.Sunday) color = Color.red;

                options.Add(
                    new OptionData
                    {
                        Text = new Timestamp(targetDate).ToString(),
                        Color = color
                    }
                );
            }
            optionsListPopup.ShowOptions(new LocalizedString("UI","creation_choose_date"), options);
            while (optionsListPopup.isActiveAndEnabled) yield return null;
            result.Value = possibleDays[optionsListPopup.LatestSelectedOption];
            var selection = result.Value;
            frontCardUI.Data.ExpirationTimestamp = new Timestamp(DateTime.Now.AddDays(selection));
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            frontCardUI.RefreshUI();
            if (autoReset) frontCardUI.AnimateToParent();
            SetSubEditMode(false);
        }

        private IEnumerator EditSubCategoryCO(CategoryDefinition categoryDef, bool autoReset = false)
        {
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
                        Text = subCategoryDef.Title.Text,
                        Color = categoryDef.Color,
                        IconText = subCategoryDef.Icon
                    }
                );
            }
            optionsListPopup.ShowOptions(new LocalizedString("UI","creation_choose_subcategory"), options);
            while (optionsListPopup.isActiveAndEnabled) yield return null;
            result.Value = subCategories[optionsListPopup.LatestSelectedOption];
            var selection = result.Value;
            frontCardUI.Data.Definition.SubCategory = selection.ID;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            frontCardUI.RefreshUI();
            if (autoReset) frontCardUI.AnimateToParent();
            SetSubEditMode(false);
        }

        public IEnumerator EditTitleCO(bool autoReset = false)
        {
            SetSubEditMode(true);
            frontCardUI.transform.localPositionTransition(new Vector3(0, -100, 0), 0.25f);
            frontCardUI.transform.localScaleTransition(Vector3.one*1.2f, 0.25f);
            frontCardUI.transform.localEulerAnglesTransform(new Vector3(0,10,0), 0.25f);
            optionsListPopup.ShowOptions(new LocalizedString("UI","creation_enter_title"), new List<OptionData>());
            yield return WaitForInputField(TitleInputField, frontCardUI.Title);
            optionsListPopup.CloseImmediate();
            frontCardUI.Data.Definition.Title.DefaultText = TitleInputField.text;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            if (autoReset) frontCardUI.AnimateToParent();
            SetSubEditMode(false);
        }

        public IEnumerator EditDescriptionCO(bool autoReset = false)
        {
            SetSubEditMode(true);
            frontCardUI.transform.localPositionTransition(new Vector3(0, 120, 0), 0.25f);
            frontCardUI.transform.localScaleTransition(Vector3.one*1.2f, 0.25f);
            frontCardUI.transform.localEulerAnglesTransform(new Vector3(0,10,0), 0.25f);
            optionsListPopup.ShowOptions(new LocalizedString("UI","creation_enter_description"), new List<OptionData>());
            yield return WaitForInputField(DescriptionInputField, frontCardUI.Description);
            optionsListPopup.CloseImmediate();
            frontCardUI.Data.Definition.Description.DefaultText = DescriptionInputField.text;
            Statics.Data.SaveProfile();
            Statics.Data.SaveCardDefinitions();
            if (autoReset) frontCardUI.AnimateToParent();
            SetSubEditMode(false);
        }

        private IEnumerator WaitForInputField(TMP_InputField inputField, UIText uiText)
        {
            inputField.textComponent = uiText;
            inputField.text = uiText.text;
            inputField.placeholder.enabled = inputField.text.IsNullOrEmpty();
            do
            {
                inputField.ActivateInputField();
                yield return null; // Must wait one frame
                while (inputField.isFocused) yield return null;
            } while (inputField.text.IsNullOrEmpty());
        }


        public void StartEditing(bool isNewCard)
        {
            var cardUI = frontCardUI;
            DescriptionInputField.textComponent = cardUI.Description;
            DescriptionInputField.text = cardUI.Description.text;
            DescriptionInputField.placeholder.enabled = DescriptionInputField.text == "";

            TitleInputField.textComponent = cardUI.Title;
            TitleInputField.text = cardUI.Title.text;
            TitleInputField.placeholder.enabled = TitleInputField.text == "";

            SetupButton(EditDifficultyButton, () => StartCoroutine(EditDifficultyCO(autoReset:true)));
            SetupButton(EditDateButton, () => StartCoroutine(EditDateCO(autoReset:true)));
            SetupButton(EditSubCategoryButton, () => StartCoroutine(EditSubCategoryCO(cardUI.Data.Definition.CategoryDefinition, autoReset:true)));
            SetupButton(EditTitleButton, () => StartCoroutine(EditTitleCO(autoReset:true)));
            SetupButton(EditDescriptionButton, () => StartCoroutine(EditDescriptionCO(autoReset:true)));

            SetupButton(DeleteCardButton, () =>
            {
                StartCoroutine(DeleteCardCO(isNewCard, frontCardUI));

            });

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
