using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Transition;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ieedo
{
    public class UICardListScreen : UIScreen
    {
        public UICardCollection CardsList;
        public GameObject FrontView;
        public RectTransform FrontViewPivot;

        public UIButton CreateCardButton;

        public UIOptionsListPopup optionsListPopup;

        [Header("Card Review")]
        public GameObject ReviewMode;
        public UIButton ValidateCardButton;

        [Header("Card View")]
        public GameObject ViewMode;
        public UIButton CompleteCardButton;
        public UIButton EditCardButton;

        [Header("Card Create & Edit")]
        public GameObject EditMode;
        public TMP_InputField DescriptionInputField;
        public TMP_InputField TitleInputField;
        public UIButton EditDifficultyButton;
        public UIButton EditDateButton;
        public UIButton EditSubCategoryButton;
        public UIButton EditTitleButton;
        public UIButton EditDescriptionButton;

        public UIButton CompleteCreationButton; // DEPRECATED
        public UIButton DeleteCardButton;
        public UIButton CloseFrontViewButton;

        public override ScreenID ID => ScreenID.CardList;
        public bool KeepPillars { get; set; }

        void Awake()
        {
            SetupButton(ValidateCardButton, () =>
            {
                CardsList.RemoveCard(frontCardUI);

                frontCardUI.Data.ValidationTimestamp = Timestamp.Now;
                frontCardUI.Data.Status = CardValidationStatus.Validated;
                Statics.Data.SaveProfile();

                StartCoroutine(ValidateCardCO(frontCardUI));
            });

            SetupButton(CompleteCardButton, () =>
            {
                CardsList.RemoveCard(frontCardUI);

                frontCardUI.Data.CompletionTimestamp = Timestamp.Now;
                frontCardUI.Data.Status = CardValidationStatus.Completed;
                Statics.Data.SaveProfile();

                StartCoroutine(CompleteCardCO(frontCardUI));
            });

            SetupButton(EditCardButton, () =>
            {
                SwitchToViewMode(FrontViewMode.Edit);
            });

            SetupButton(CreateCardButton, () =>
            {
                StartCoroutine(CardCreationFlowCO());
            });

            CardsList.OnCardClicked = uiCard => OpenFrontView(uiCard, CurrentListViewMode == ListViewMode.ToDo ? FrontViewMode.Edit : FrontViewMode.Review);
        }

        private IEnumerator ValidateCardCO(UICard uiCard)
        {
            uiCard.transform.localPositionTransition(new Vector3(-300,600,-150), 0.5f, LeanEase.Accelerate);
            uiCard.transform.localEulerAnglesTransform(new Vector3(0,-20,-20), 0.5f, LeanEase.Accelerate);
            yield return new WaitForSeconds(0.5f);
            if (uiCard != null) Destroy(uiCard.gameObject);
            CloseFrontView();
            GoTo(ScreenID.Pillars);
            Statics.Score.AddScore(50);
        }


        private IEnumerator CompleteCardCO(UICard uiCard)
        {
            uiCard.transform.localPositionTransition(new Vector3(-300,600,-150), 0.5f, LeanEase.Accelerate);
            uiCard.transform.localEulerAnglesTransform(new Vector3(0,-20,-20), 0.5f, LeanEase.Accelerate);
            yield return new WaitForSeconds(0.5f);
            if (uiCard != null) Destroy(uiCard.gameObject);
            CloseFrontView();
            GoTo(ScreenID.Pillars);
            Statics.Score.AddScore(20);
        }

        protected override IEnumerator OnOpen()
        {
            FrontView.gameObject.SetActive(false);
            //Statics.Data.LoadCardDefinitions();

            switch (CurrentListViewMode)
            {
                case ListViewMode.Review:
                    CreateCardButton.gameObject.SetActive(false);
                    break;
                case ListViewMode.ToDo:
                    CreateCardButton.gameObject.SetActive(true);
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

        public void LoadCards(List<CardData> cards, Func<CardData,CardData,int> sort, ListViewMode mode)
        {
            CurrentListViewMode = mode;
            var collection = new CardDataCollection();
            collection.AddRange(cards);
            CardsList.AssignList(collection);
            CardsList.SortList(sort);
        }

        public void LoadToDoCards()
        {
            var cards = Statics.Data.Profile.Cards.Where(x => x.Status == CardValidationStatus.Todo).ToList();
            LoadCards(cards, SortByExpirationDate, ListViewMode.ToDo);
        }

        public enum ListViewMode
        {
            ToDo,
            Review,
        }

        public enum FrontViewMode
        {
            None,
            View,
            Create,
            Edit,
            Review
        }

        public ListViewMode CurrentListViewMode;
        private FrontViewMode CurrentFrontViewMode;

        private void OpenFrontView(UICard uiCard, FrontViewMode viewMode)
        {
            prevFrontCardParent = uiCard.transform.parent;
            var uiCardRt = uiCard.GetComponent<RectTransform>();
            uiCardRt.SetParent(FrontViewPivot, true);
            uiCardRt.anchorMin = new Vector2(0.5f, 0.5f);
            uiCardRt.anchorMax = new Vector2(0.5f, 0.5f);
            uiCard.AnimateToParent();

            FrontView.gameObject.SetActive(true);
            frontCardUI = uiCard;

            SwitchToViewMode(viewMode);
        }

        private void SwitchToViewMode(FrontViewMode viewMode)
        {
            CurrentFrontViewMode = viewMode;
            switch (viewMode)
            {
                case FrontViewMode.Edit:
                    EditMode.SetActive(true);
                    ViewMode.SetActive(false);
                    ReviewMode.SetActive(false);
                    StartEditing(false);
                    break;
                case FrontViewMode.Create:
                    EditMode.SetActive(true);
                    ViewMode.SetActive(false);
                    ReviewMode.SetActive(false);
                    StartEditing(true);
                    break;
                case FrontViewMode.View:
                    EditMode.SetActive(false);
                    ViewMode.SetActive(true);
                    ReviewMode.SetActive(false);
                    break;
                case FrontViewMode.Review:
                    EditMode.SetActive(false);
                    ViewMode.SetActive(false);
                    ReviewMode.SetActive(true);
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

        public IEnumerator CardCreationFlowCO()
        {
            EditSubCategoryButton.gameObject.SetActive(false);
            EditDifficultyButton.gameObject.SetActive(false);
            EditDateButton.gameObject.SetActive(false);
            EditTitleButton.gameObject.SetActive(false);
            EditDescriptionButton.gameObject.SetActive(false);

            // Create and show the card
            var cardDef = Statics.Cards.GenerateCardDefinition(
                new CardDefinition {
                    Category = CategoryID.None,
                    SubCategory = 0,
                    Description = new LocalizedString { DefaultText = "" },
                    Difficulty = 0,
                    Title = new LocalizedString { DefaultText = ""},
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
            Statics.Cards.AssignCard(cardData);
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

            EditSubCategoryButton.gameObject.SetActive(true);
            EditDifficultyButton.gameObject.SetActive(true);
            EditDateButton.gameObject.SetActive(true);
            EditDescriptionButton.gameObject.SetActive(true);
            EditTitleButton.gameObject.SetActive(true);
        }

        private IEnumerator EditCategoryCO(bool autoReset = false)
        {
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
            optionsListPopup.ShowOptions("Choose Category", options);
            while (optionsListPopup.isActiveAndEnabled) yield return null;
            catResult.Value = categories[optionsListPopup.LatestSelectedOption];
            var selectedCategory = catResult.Value;
            frontCardUI.Data.Definition.Category = selectedCategory.ID;
            Statics.Data.SaveProfile();
            frontCardUI.RefreshUI();
            if (autoReset) frontCardUI.AnimateToParent();
        }

        private IEnumerator EditDifficultyCO(bool autoReset = false)
        {
            var result = new Ref<int>();
            frontCardUI.transform.localPositionTransition(new Vector3(0, 150, 0), 0.25f);
            var options = new List<OptionData>();
            var possibleDifficulties = new[] { 1, 2, 3 };
            foreach (var possibleDifficulty in possibleDifficulties)
            {
                options.Add(
                    new OptionData
                    {
                        Text = possibleDifficulty.ToString(),
                        Color = Color.white,
                    }
                );
            }
            optionsListPopup.ShowOptions("Choose Difficulty", options);
            while (optionsListPopup.isActiveAndEnabled) yield return null;
            result.Value = possibleDifficulties[optionsListPopup.LatestSelectedOption];
            var selection = result.Value;
            frontCardUI.Data.Definition.Difficulty = selection;
            Statics.Data.SaveProfile();
            frontCardUI.RefreshUI();
            if (autoReset) frontCardUI.AnimateToParent();
        }

        private IEnumerator EditDateCO(bool autoReset = false)
        {
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
            optionsListPopup.ShowOptions("Choose Date", options);
            while (optionsListPopup.isActiveAndEnabled) yield return null;
            result.Value = possibleDays[optionsListPopup.LatestSelectedOption];
            var selection = result.Value;
            frontCardUI.Data.ExpirationTimestamp = new Timestamp(DateTime.Now.AddDays(selection));
            Statics.Data.SaveProfile();
            frontCardUI.RefreshUI();
            if (autoReset) frontCardUI.AnimateToParent();
        }

        private IEnumerator EditSubCategoryCO(CategoryDefinition categoryDef, bool autoReset = false)
        {
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
                        Color = categoryDef.Color
                    }
                );
            }
            optionsListPopup.ShowOptions("Choose Sub-category", options);
            while (optionsListPopup.isActiveAndEnabled) yield return null;
            result.Value = subCategories[optionsListPopup.LatestSelectedOption];
            var selection = result.Value;
            frontCardUI.Data.Definition.SubCategory = selection.ID;
            Statics.Data.SaveProfile();
            frontCardUI.RefreshUI();
            if (autoReset) frontCardUI.AnimateToParent();
        }

        public IEnumerator EditTitleCO(bool autoReset = false)
        {
            frontCardUI.transform.localPositionTransition(new Vector3(0, -100, 0), 0.25f);
            frontCardUI.transform.localScaleTransition(Vector3.one*1.2f, 0.25f);
            frontCardUI.transform.localEulerAnglesTransform(new Vector3(0,10,0), 0.25f);
            optionsListPopup.ShowOptions("Enter Title", new List<OptionData>());
            yield return WaitForInputField(TitleInputField, frontCardUI.Title);
            optionsListPopup.CloseImmediate();
            frontCardUI.Data.Definition.Title.DefaultText = TitleInputField.text;
            Statics.Data.SaveProfile();
            if (autoReset) frontCardUI.AnimateToParent();
        }

        public IEnumerator EditDescriptionCO(bool autoReset = false)
        {
            frontCardUI.transform.localPositionTransition(new Vector3(0, 200, 0), 0.25f);
            frontCardUI.transform.localScaleTransition(Vector3.one*1.2f, 0.25f);
            frontCardUI.transform.localEulerAnglesTransform(new Vector3(0,10,0), 0.25f);
            optionsListPopup.ShowOptions("Enter Description", new List<OptionData>());
            yield return WaitForInputField(DescriptionInputField, frontCardUI.Description);
            optionsListPopup.CloseImmediate();
            frontCardUI.Data.Definition.Description.DefaultText = DescriptionInputField.text;
            Statics.Data.SaveProfile();
            if (autoReset) frontCardUI.AnimateToParent();
        }

        private IEnumerator WaitForInputField(TMP_InputField inputField, UIText uiText)
        {
            inputField.textComponent = uiText;
            inputField.text = uiText.text;
            inputField.placeholder.enabled = inputField.text == "";
            inputField.ActivateInputField();
            yield return null; // Wait one frame
            while (inputField.isFocused) yield return null;
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

            SetupButton(CompleteCreationButton, () =>
            {
                StopEditing();
                CloseFrontView();

                if (isNewCard) CardsList.AddCard(cardUI.Data, cardUI);
                CardsList.SortList(SortByExpirationDate);

                cardUI.RefreshUI();
            });

            SetupButton(CloseFrontViewButton, () =>
            {
                bool canComplete = !frontCardUI.Description.text.IsNullOrEmpty() && !frontCardUI.Title.text.IsNullOrEmpty();
                if (!canComplete) return;

                StopEditing();
                CloseFrontView();

                if (isNewCard) CardsList.AddCard(cardUI.Data, cardUI);
                CardsList.SortList(SortByExpirationDate);

                cardUI.RefreshUI();
            });

            SetupButton(DeleteCardButton, () =>
            {
                if (!isNewCard) CardsList.RemoveCard(frontCardUI);
                StopEditing();
                CloseFrontView();

                Statics.Cards.DeleteCard(cardUI.Data);
                Statics.Cards.DeleteCardDefinition(cardUI.Data.Definition);
            });

            StartCoroutine(EditModeCO());
        }

        private IEnumerator EditModeCO()
        {
            // Wait for closing...
            while (CurrentFrontViewMode != FrontViewMode.None)
            {
                //bool canComplete = !frontCardUI.Description.text.IsNullOrEmpty() && !frontCardUI.Title.text.IsNullOrEmpty();
                //CompleteCreationButton.gameObject.SetActive(canComplete);   // TODO: instead use interactable

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