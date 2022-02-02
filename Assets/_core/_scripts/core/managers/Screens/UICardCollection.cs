using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{
    public class UICardCollection : MonoBehaviour
    {
        public RectTransform CardsPivot;
        public int CardScale = 3;

        public Action<UICard> OnCardClicked;

        public void AssignList(List<CardDefinition> cardsList)
        {
            foreach (var cardDef in cardsList)
            {
                AssignCard(cardDef);
            }
        }

        public UICard AssignCard(CardDefinition cardDef)
        {
            var uiCard = UICardManager.I.AddCardUI(cardDef, CardsPivot);
            uiCard.transform.localScale = Vector3.one * CardScale;
            uiCard.OnInteraction(() => OnCardClicked(uiCard));
            return uiCard;
        }

    }
}