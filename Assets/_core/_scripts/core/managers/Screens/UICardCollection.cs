using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ieedo
{
    public class UICardCollection : MonoBehaviour
    {
        public RectTransform SlotPrefab;
        public int CardScale = 3;

        public Action<UICard> OnCardClicked;

        private List<GameObject> HeldSlots = new List<GameObject>();

        public void AssignList(List<CardDefinition> cardsList)
        {
            HeldSlots.ForEach(x => Destroy(x.gameObject));
            HeldSlots.Clear();

            foreach (var cardDef in cardsList)
            {
                AssignCard(cardDef);
            }
        }

        public UICard AssignCard(CardDefinition cardDef, UICard uiCard = null)
        {
            // Copy a slot
            var newSlotRT = Instantiate(SlotPrefab, SlotPrefab.parent);
            newSlotRT.gameObject.SetActive(true);
            newSlotRT.transform.localScale = Vector3.one * CardScale;

            if (uiCard == null) uiCard = UICardManager.I.AddCardUI(cardDef, newSlotRT);
            else uiCard.transform.SetParent(newSlotRT);
            PutCard(uiCard);
            HeldSlots.Add(newSlotRT.gameObject);
            return uiCard;
        }

        public void PutCard(UICard uiCard)
        {
            uiCard.OnInteraction(() => OnCardClicked(uiCard));
        }

    }
}