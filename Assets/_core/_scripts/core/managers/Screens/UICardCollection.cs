using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{
    public class UICardCollection : MonoBehaviour
    {
        public int CardScale = 3;

        public Action<UICard> OnCardClicked;

        public void AssignList(List<CardDefinition> cardsList)
        {
            foreach (var cardDef in cardsList)
            {
                var uiCard = UICardManager.I.AddCardUI(cardDef, transform);
                uiCard.transform.localScale = Vector3.one * CardScale;
                uiCard.InteractionButton.onClick.AddListener(() => OnCardClicked(uiCard));
            }
        }

    }
}