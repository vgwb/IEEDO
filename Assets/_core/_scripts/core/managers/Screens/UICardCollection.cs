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
        private List<UICard> HeldCards = new List<UICard>();

        public void AssignList(List<CardData> cardsList)
        {
            HeldSlots.ForEach(x => Destroy(x.gameObject));
            HeldSlots.Clear();
            HeldCards.Clear();

            foreach (var cardDef in cardsList)
            {
                AddCard(cardDef);
            }
        }

        public UICard AddCard(CardData cardData, UICard uiCard = null)
        {
            // Copy a slot
            var newSlotRT = Instantiate(SlotPrefab, SlotPrefab.parent);
            newSlotRT.gameObject.SetActive(true);
            newSlotRT.transform.localScale = Vector3.one * CardScale;

            if (uiCard == null) uiCard = UICardManager.I.AddCardUI(cardData, newSlotRT);
            else uiCard.transform.SetParent(newSlotRT);
            PutCard(uiCard);
            HeldSlots.Add(newSlotRT.gameObject);
            HeldCards.Add(uiCard);
            return uiCard;
        }

        public void SortList()
        {

        }

        public void RemoveCard(UICard uiCard)
        {
            var index = HeldCards.IndexOf(uiCard);
            Destroy(HeldCards[index]);
            HeldCards.RemoveAt(index);

            Destroy(HeldSlots[index]);
            HeldSlots.RemoveAt(index);
        }

        public void PutCard(UICard uiCard)
        {
            uiCard.OnInteraction(() => OnCardClicked(uiCard));
        }

        public void SortList(Func<CardData, CardData, int> sortFunc)
        {
            HeldCards.Sort((ui1, ui2) => sortFunc(ui1.Data, ui2.Data));
            for (int i = 0; i < HeldCards.Count; i++)
            {
                // Move to the correct slot (TODO: animate the sorting)
                HeldCards[i].transform.SetParent(HeldSlots[i].transform);
                HeldCards[i].transform.localScale = Vector3.one;
                HeldCards[i].transform.localPosition = Vector3.zero;
            }
        }
    }
}