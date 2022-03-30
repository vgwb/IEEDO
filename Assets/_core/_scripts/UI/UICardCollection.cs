using System;
using System.Collections.Generic;
using Lean.Transition;
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
        public List<UICard> HeldCards = new List<UICard>();

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
            if (HeldCards.Contains(uiCard)) return uiCard;

            // Copy a slot
            var newSlotRT = Instantiate(SlotPrefab, SlotPrefab.parent);
            newSlotRT.name = $"Slot{HeldSlots.Count}";
            newSlotRT.gameObject.SetActive(true);
            newSlotRT.localScale = Vector3.one * CardScale;
            if (uiCard == null) uiCard = UICardManager.I.CreateCardUI(cardData, newSlotRT);
            else uiCard.transform.SetParent(newSlotRT, true);

            uiCard.AnimateToParent();

            SetupInListInteraction(uiCard);
            HeldSlots.Add(newSlotRT.gameObject);
            HeldCards.Add(uiCard);
            return uiCard;
        }

        public void RemoveCard(UICard uiCard)
        {
            var index = HeldCards.IndexOf(uiCard);
            if (index >= 0)
            {
                //Destroy(HeldCards[index].gameObject);
                HeldCards.RemoveAt(index);

                Destroy(HeldSlots[index].gameObject);
                HeldSlots.RemoveAt(index);
            }
        }

        public void SetupInListInteraction(UICard uiCard)
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

        public void AnimateEntrance(UICardListScreen.ListViewMode currentListViewMode)
        {
            for (var iCard = 0; iCard < HeldCards.Count; iCard++)
            {
                UICard heldCard = HeldCards[iCard];

                switch (currentListViewMode)
                {
                    case UICardListScreen.ListViewMode.ToDo:
                        heldCard.transform.localPosition = Vector3.left * 400;
                        break;
                    case UICardListScreen.ListViewMode.Pillars:
                        heldCard.transform.localPosition = Vector3.up * 1500;
                        break;
                }

                heldCard.transform.localEulerAnglesTransform(new Vector3(0, 0f, 0f), 0f); // fake transition to trigger the delay correctly
                heldCard.transform.JoinDelayTransition((HeldCards.Count - 1 - iCard) * 0.1f).localPositionTransition(Vector3.zero, 0.5f);

                if (currentListViewMode == UICardListScreen.ListViewMode.ToDo)
                {
                    HeldSlots[iCard].transform.localEulerAnglesTransform(new Vector3(0, 0f, -5f), 0.5f);
                }
            }
        }
    }
}
