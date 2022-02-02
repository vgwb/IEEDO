using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Ieedo
{
    public class UIToDoListScreen : UIScreen
    {
        public UICardCollection ToDoList;
        public GameObject FrontView;
        public RectTransform FrontViewPivot;

        public LeanButton CompleteCardButton;
        public LeanButton CreateCardButton;

        public UIOptionsList OptionsList;

        public override ScreenID ID => ScreenID.ToDoList;

        protected override IEnumerator OnOpen()
        {
            FrontView.gameObject.SetActive(false);

            LoadCurrentCards();
            yield return base.OnOpen();
        }

        protected override IEnumerator OnClose()
        {
            CloseFrontView();
            yield return base.OnClose();
        }

        public UICard frontCard;

        public void LoadCurrentCards()
        {
            Statics.Data.LoadCards();

            // TODO: add a timing handling, sort based on timing
            var todoCards = Statics.Data.Cards;
            //todoCards.Sort((c1, c2) => c1.Category - c2.Category);

            ToDoList.AssignList(todoCards);
            ToDoList.OnCardClicked = uiCard => OpenFrontView(uiCard, FrontViewMode.View);

            SetupButton(CompleteCardButton, () =>
            {
                Destroy(frontCard);
                Statics.Cards.DeleteCard(frontCard.Definition);

                CloseFrontView();
            });

            SetupButton(CreateCardButton, () =>
            {
                StartCoroutine(CardCreationFlowCO());
            });
        }

        public enum FrontViewMode
        {
            None,
            View,
            Creation
        }

        private FrontViewMode CurrentFrontViewMode;
        private void OpenFrontView(UICard uiCard, FrontViewMode viewMode)
        {
            CurrentFrontViewMode = viewMode;
            var prevParent = uiCard.transform.parent;
            var uiCardRt = uiCard.GetComponent<RectTransform>();
            uiCardRt.SetParent(FrontViewPivot, false);
            uiCardRt.anchorMin = new Vector2(0.5f, 0.5f);
            uiCardRt.anchorMax = new Vector2(0.5f, 0.5f);
            uiCardRt.anchoredPosition = Vector3.zero;
            FrontView.gameObject.SetActive(true);
            frontCard = uiCard;

            switch (viewMode)
            {
                case FrontViewMode.Creation:
                    DescriptionInputField.gameObject.SetActive(true);
                    TitleInputField.gameObject.SetActive(true);
                    CompleteCardButton.gameObject.SetActive(false);
                    CompleteCreationButton.gameObject.SetActive(true);
                    uiCard.OnInteraction(null);
                    break;
                case FrontViewMode.View:
                    DescriptionInputField.gameObject.SetActive(false);
                    TitleInputField.gameObject.SetActive(false);
                    CompleteCardButton.gameObject.SetActive(true);
                    CompleteCreationButton.gameObject.SetActive(false);
                    uiCard.OnInteraction(() =>
                    {
                        // Return it the list
                        uiCard.transform.SetParent(prevParent, false);
                        CloseFrontView();
                    });
                    break;
            }
        }

        private void CloseFrontView()
        {
            FrontView.gameObject.SetActive(false);

            if (CurrentFrontViewMode == FrontViewMode.View && frontCard != null)
            {
                ToDoList.PutCard(frontCard);
            }
            frontCard = null;
            CurrentFrontViewMode = FrontViewMode.None;
        }


        #region Card Creation

        [Header("Card Creation")]
        public TMP_InputField DescriptionInputField;
        public TMP_InputField TitleInputField;
        public UIButton CompleteCreationButton;

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
            OptionsList.ShowOptions(options);
            while (OptionsList.isActiveAndEnabled) yield return null;
            var selectedCategory = categories[OptionsList.LatestSelectedOption];

            // Choose sub-category
            options.Clear();
            var subCategories = selectedCategory.SubCategories;
            foreach (var subCategoryDef in subCategories)
            {
                options.Add(
                    new OptionData
                    {
                        Text = subCategoryDef.Title.Text,
                        Color = selectedCategory.Color
                    }
                );
            }
            OptionsList.ShowOptions(options);
            while (OptionsList.isActiveAndEnabled) yield return null;
            var selectedSybCategory = subCategories[OptionsList.LatestSelectedOption];

            // Create and show the card
            var card = Statics.Cards.GenerateCard(
                new CardDefinition {
                    Category = selectedCategory.ID,
                    SubCategory = selectedSybCategory.ID,
                    Description = new LocalizedString { DefaultText = "" },
                    Difficulty = 2,
                    Title = new LocalizedString { DefaultText = ""}
                });
            var cardUi = UICardManager.I.AddCardUI(card, FrontViewPivot);
            OpenFrontView(cardUi, FrontViewMode.Creation);

            // Allow card editing...
            DescriptionInputField.textComponent = cardUi.Description;
            DescriptionInputField.placeholder.enabled = true;

            TitleInputField.textComponent = cardUi.Title;
            TitleInputField.placeholder.enabled = true;

            bool hasClosed = false;
            SetupButton(CompleteCreationButton, () =>
            {
                ToDoList.AssignCard(card);
                CloseFrontView();
                hasClosed = true;
            });

            while (!hasClosed) yield return null; // Wait for completion

            TitleInputField.textComponent = null;
            TitleInputField.text = string.Empty;
            DescriptionInputField.textComponent = null;
            DescriptionInputField.text = string.Empty;
        }

        #endregion

    }
}