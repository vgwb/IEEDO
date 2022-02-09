using System;
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

        public UIButton CreateCardButton;

        public UIOptionsList OptionsList;

        [Header("Card View")]
        public GameObject ViewMode;
        public UIButton ValidateCardButton;
        public UIButton EditCardButton;

        [Header("Card Create & Edit")]
        public GameObject EditMode;
        public TMP_InputField DescriptionInputField;
        public TMP_InputField TitleInputField;
        public UIButton CompleteCreationButton;
        public UIButton DeleteCardButton;

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
            Statics.Data.LoadCardDefinitions();

            // TODO: add a timing handling, sort based on timing
            var todoCards = Statics.Data.Profile.Cards;
            //todoCards.Sort((c1, c2) => c1.Category - c2.Category);

            ToDoList.AssignList(todoCards);
            ToDoList.OnCardClicked = uiCard => OpenFrontView(uiCard, FrontViewMode.View);

            SetupButton(ValidateCardButton, () =>
            {
                ToDoList.RemoveCard(frontCard);
                Statics.Cards.DeleteCard(frontCard.Data);

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
            CreateAndEdit
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
                case FrontViewMode.CreateAndEdit:
                    EditMode.SetActive(true);
                    ViewMode.SetActive(false);
                    uiCard.OnInteraction(null);
                    break;
                case FrontViewMode.View:
                    EditMode.SetActive(false);
                    ViewMode.SetActive(true);
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

            // Choose difficulty
            options.Clear();
            var possibleDifficulties = new int[] { 1, 2, 3 };
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
            OptionsList.ShowOptions(options);
            while (OptionsList.isActiveAndEnabled) yield return null;
            var selectedDifficulty = possibleDifficulties[OptionsList.LatestSelectedOption];

            // Choose Date
            options.Clear();
            var possibleDays = new [] { 1, 2, 3, 4, 5, 6 };
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
            OptionsList.ShowOptions(options);
            while (OptionsList.isActiveAndEnabled) yield return null;
            var selectedDays = possibleDays[OptionsList.LatestSelectedOption];

            // Create and show the card
            var cardDef = Statics.Cards.GenerateCardDefinition(
                new CardDefinition {

                    Category = selectedCategory.ID,
                    SubCategory = selectedSybCategory.ID,
                    Description = new LocalizedString { DefaultText = "" },
                    Difficulty = selectedDifficulty,
                    Title = new LocalizedString { DefaultText = ""},
                });

            // Create a new Data for this profile for that card
            var cardData = new CardData
            {
                DefID = cardDef.UID,
                CreationDate = new Timestamp(DateTime.Now),
                ExpirationDate = new Timestamp(DateTime.Now.AddDays(selectedDays))
            };
            Statics.Cards.AssignCard(cardData);

            var cardUi = UICardManager.I.AddCardUI(cardData, FrontViewPivot);
            OpenFrontView(cardUi, FrontViewMode.CreateAndEdit);

            // Allow card editing...
            DescriptionInputField.textComponent = cardUi.Description;
            DescriptionInputField.placeholder.enabled = true;

            TitleInputField.textComponent = cardUi.Title;
            TitleInputField.placeholder.enabled = true;

            bool hasClosed = false;

            SetupButton(CompleteCreationButton, () =>
            {
                ToDoList.AssignCard(cardData);
                CloseFrontView();
                hasClosed = true;
            });

            SetupButton(DeleteCardButton, () =>
            {
                Statics.Cards.DeleteCard(cardData);
                Statics.Cards.DeleteCardDefinition(cardDef);
                CloseFrontView();
                hasClosed = true;
            });

            while (!hasClosed)
            {
                CompleteCreationButton.enabled =  cardUi.Description.text != "" && cardUi.Title.text != "";
                yield return null; // Wait for completion
            }

            TitleInputField.textComponent = null;
            TitleInputField.text = string.Empty;
            DescriptionInputField.textComponent = null;
            DescriptionInputField.text = string.Empty;
        }

        #endregion

    }
}