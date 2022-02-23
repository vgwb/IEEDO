using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Ieedo.Utilities;

namespace Ieedo
{
    public class UICardListScreen : UIScreen
    {
        public override bool AutoAnimate => false;

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

        public UIButton CompleteCreationButton;
        public UIButton DeleteCardButton;

        public override ScreenID ID => ScreenID.CardList;

        void Awake()
        {
            SetupButton(ValidateCardButton, () =>
            {
                CardsList.RemoveCard(frontCardUI);

                frontCardUI.Data.ValidationTimestamp = Timestamp.Now;
                frontCardUI.Data.Status = CardValidationStatus.Validated;
                Statics.Data.SaveProfile();

                CloseFrontView();
            });

            SetupButton(CompleteCardButton, () =>
            {
                CardsList.RemoveCard(frontCardUI);

                frontCardUI.Data.CompletionTimestamp = Timestamp.Now;
                frontCardUI.Data.Status = CardValidationStatus.Completed;
                Statics.Data.SaveProfile();

                CloseFrontView();
            });

            SetupButton(EditCardButton, () =>
            {
                SwitchToViewMode(FrontViewMode.Edit);
            });

            SetupButton(CreateCardButton, () =>
            {
                StartCoroutine(CardCreationFlowCO());
            });

            CardsList.OnCardClicked = uiCard => OpenFrontView(uiCard, CurrentFrontViewMode);
        }

        protected override IEnumerator OnOpen()
        {
            FrontView.gameObject.SetActive(false);
            Statics.Data.LoadCardDefinitions();

            //LoadToDoCards();
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

        public void LoadCards(List<CardData> cards, Func<CardData,CardData,int> sort)
        {
            var collection = new CardDataCollection();
            collection.AddRange(cards);
            CardsList.AssignList(collection);
            CardsList.SortList(sort);
        }

        public void LoadToDoCards()
        {
            var cards = Statics.Data.Profile.Cards.Where(x => x.Status == CardValidationStatus.Todo).ToList();
            LoadCards(cards, SortByExpirationDate);
        }

        public enum FrontViewMode
        {
            None,
            View,
            Create,
            Edit,
            Review
        }

        private FrontViewMode CurrentFrontViewMode;
        private void OpenFrontView(UICard uiCard, FrontViewMode viewMode)
        {
            prevFrontCardParent = uiCard.transform.parent;
            var uiCardRt = uiCard.GetComponent<RectTransform>();
            uiCardRt.SetParent(FrontViewPivot, false);
            uiCardRt.localEulerAngles = Vector3.zero;
            uiCardRt.anchorMin = new Vector2(0.5f, 0.5f);
            uiCardRt.anchorMax = new Vector2(0.5f, 0.5f);
            uiCardRt.anchoredPosition = Vector3.zero;
            FrontView.gameObject.SetActive(true);
            frontCardUI = uiCard;

            uiCard.OnInteraction(() =>
            {
                // Return it the list on click
                if (CurrentFrontViewMode == FrontViewMode.View)
                {
                    CloseFrontView();
                }
            });

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
                    EditMode.SetActive(true);
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
                if (prevFrontCardParent != null) frontCardUI.transform.SetParent(prevFrontCardParent, false);
                CardsList.SetupInListInteraction(frontCardUI);
            }
            frontCardUI = null;
            prevFrontCardParent = null;
            CurrentFrontViewMode = FrontViewMode.None;
        }


        #region Card Creation & Editing

        public IEnumerator CardCreationFlowCO()
        {
            // Choose category
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
            var selectedCategory = categories[optionsListPopup.LatestSelectedOption];

            // Choose sub-category
            var subResult = new Ref<SubCategoryDefinition>();
            yield return ChooseSubCategoryCO(subResult, selectedCategory);
            var selectedSubCategory = subResult.Value;

            // Choose difficulty
            var result = new Ref<int>();
            yield return ChooseDifficultyCO(result);
            var selectedDifficulty = result.Value;

            // Choose Date
            yield return ChooseDateCO(result);
            var selectedDays = result.Value;

            // Create and show the card
            var cardDef = Statics.Cards.GenerateCardDefinition(
                new CardDefinition {

                    Category = selectedCategory.ID,
                    SubCategory = selectedSubCategory.ID,
                    Description = new LocalizedString { DefaultText = "" },
                    Difficulty = selectedDifficulty,
                    Title = new LocalizedString { DefaultText = ""},
                },
                isDefaultCard: AppManager.I.ApplicationConfig.SaveCardsAsDefault
                );

            // Create a new Data for this profile for that card
            var cardData = new CardData
            {
                DefID = cardDef.UID,
                CreationTimestamp = new Timestamp(DateTime.Now),
                ExpirationTimestamp = new Timestamp(DateTime.Now.AddDays(selectedDays))
            };
            Statics.Cards.AssignCard(cardData);
            var cardUi = UICardManager.I.AddCardUI(cardData, FrontViewPivot);

            OpenFrontView(cardUi, FrontViewMode.Create);

        }

        private IEnumerator ChooseSubCategoryCO(Ref<SubCategoryDefinition> result, CategoryDefinition category)
        {
            var options = new List<OptionData>();
            var subCategories = category.SubCategories;
            foreach (var subCategoryDef in subCategories)
            {
                options.Add(
                    new OptionData
                    {
                        Text = subCategoryDef.Title.Text,
                        Color = category.Color
                    }
                );
            }
            optionsListPopup.ShowOptions("Choose Sub-category", options);
            while (optionsListPopup.isActiveAndEnabled) yield return null;
            result.Value = subCategories[optionsListPopup.LatestSelectedOption];

        }

        private IEnumerator ChooseDifficultyCO(Ref<int> result)
        {
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
        }


        private IEnumerator ChooseDateCO(Ref<int> result)
        {
            var options = new List<OptionData>();
            var possibleDays = new [] { 0, 1, 2, 3, 4, 5, 6 };
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

            SetupButton(EditDifficultyButton, () => StartCoroutine(EditDifficultyCO()));
            SetupButton(EditDateButton, () => StartCoroutine(EditDateCO()));
            SetupButton(EditSubCategoryButton, () => StartCoroutine(EditSubCategoryCO(cardUI.Data.Definition.CategoryDefinition)));

            SetupButton(CompleteCreationButton, () =>
            {
                StopEditing();

                if (isNewCard) CardsList.AddCard(cardUI.Data);
                CardsList.SortList(SortByExpirationDate);

                CloseFrontView();
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
                bool canComplete = !frontCardUI.Description.text.IsNullOrEmpty() && !frontCardUI.Title.text.IsNullOrEmpty();
                CompleteCreationButton.gameObject.SetActive(canComplete);   // TODO: instead use interactable

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

        private IEnumerator EditDifficultyCO()
        {
            var result = new Ref<int>();
            yield return ChooseDifficultyCO(result);
            var selection = result.Value;
            frontCardUI.Data.Definition.Difficulty = selection;
            frontCardUI.RefreshUI();
        }

        private IEnumerator EditDateCO()
        {
            var result = new Ref<int>();
            yield return ChooseDateCO(result);
            var selection = result.Value;
            frontCardUI.Data.ExpirationTimestamp = new Timestamp(DateTime.Now.AddDays(selection));
            frontCardUI.RefreshUI();
        }

        private IEnumerator EditSubCategoryCO(CategoryDefinition categoryDefinition)
        {
            var result = new Ref<SubCategoryDefinition>();
            yield return ChooseSubCategoryCO(result, categoryDefinition);
            var selection = result.Value;
            frontCardUI.Data.Definition.SubCategory = selection.ID;
            frontCardUI.RefreshUI();
        }
        #endregion

    }
}