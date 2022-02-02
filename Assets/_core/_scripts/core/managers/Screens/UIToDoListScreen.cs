using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;

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
            // TODO: close the current card
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
            ToDoList.OnCardClicked = OpenFrontView;

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

        private void OpenFrontView(UICard uiCard)
        {
            var prevParent = uiCard.transform.parent;
            uiCard.transform.SetParent(FrontViewPivot, false);
            uiCard.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            uiCard.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            uiCard.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            FrontView.gameObject.SetActive(true);
            frontCard = uiCard;

            uiCard.OnInteraction(() =>
            {
                // Return it the list
                uiCard.transform.SetParent(prevParent, false);

                CloseFrontView();
            });
        }

        private void CloseFrontView()
        {
            FrontView.gameObject.SetActive(false);
            frontCard = null;
        }


        #region Card Creation

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

            var card = Statics.Cards.GenerateCard(
                new CardDefinition {
                    Category = selectedCategory.ID,
                    SubCategory = selectedSybCategory.ID,
                    Description = new LocalizedString { DefaultText = "" },
                    Difficulty = 2,
                    Title = new LocalizedString { DefaultText = ""}
                });
            var cardUi = ToDoList.AssignCard(card);
            OpenFrontView(cardUi);
        }

        #endregion

    }
}