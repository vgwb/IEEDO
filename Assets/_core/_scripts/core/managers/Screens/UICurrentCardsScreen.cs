using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{
    public class UICurrentCardsScreen : UIScreen
    {
        public UICardCollection ToDoList;
        public RectTransform FrontViewPivot;

        public override ScreenID ID => ScreenID.CurrentCards;

        protected override IEnumerator OnOpen()
        {
            LoadCurrentCards();
            yield return base.OnOpen();
        }

        public void LoadCurrentCards()
        {
            Statics.Data.LoadCards();

            // TODO: add a timing handling
            var todoCards = Statics.Data.Cards;

            ToDoList.AssignList(todoCards);
            ToDoList.OnCardClicked = uiCard =>
            {
                uiCard.transform.SetParent(FrontViewPivot, false);
            };
        }
    }
}