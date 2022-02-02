using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;

namespace Ieedo
{
    public class UICurrentCardsScreen : UIScreen
    {
        public UICardCollection ToDoList;
        public GameObject FrontView;
        public RectTransform FrontViewPivot;

        public LeanButton CompleteCardButton;
        public LeanButton CreateCardButton;

        public override ScreenID ID => ScreenID.CurrentCards;

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
                var card = Statics.Cards.GenerateCard(
                    new CardDefinition {
                        Category = (CategoryID)UnityEngine.Random.Range(1, 4),
                        Description = new LocalizedString() { DefaultText = "TEST" + UnityEngine.Random.Range(0, 50) },
                        Difficulty = (uint)UnityEngine.Random.Range(1, 5),
                        Title = new LocalizedString() { DefaultText = "TEST" + UnityEngine.Random.Range(0, 50) },
                    });
                var cardUi = ToDoList.AssignCard(card);
                OpenFrontView(cardUi);
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

    }
}