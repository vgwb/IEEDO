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
        public SnappingScrollRect ScrollRect;
        public float CardScale = 3;

        public Action<UICard> OnCardClicked;

        public List<GameObject> HeldSlots = new List<GameObject>();
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
            var targetParent = newSlotRT;
            if (uiCard == null)
            {
                uiCard = UICardManager.I.CreateCardUI(cardData, targetParent);
            }
            else
            {
                uiCard.transform.SetParent(targetParent, true);
            }

            uiCard.AnimateToParent();

            SetupInListInteraction(uiCard);
            HeldSlots.Add(newSlotRT.gameObject);
            HeldCards.Add(uiCard);
            return uiCard;
        }

        public void RemoveCard(UICard uiCard)
        {
            var index = HeldCards.IndexOf(uiCard);
            if (index < 0) return;

            HeldCards.RemoveAt(index);

            SortListAgain();
        }

        public void SetupInListInteraction(UICard uiCard)
        {
            uiCard.OnInteraction(() => OnCardClicked(uiCard));
        }

        private Func<CardData, CardData, int> currentSortFunc;

        public void SortListAgain()
        {
            SortList(currentSortFunc);
        }

        public void SortList(Func<CardData, CardData, int> sortFunc)
        {
            currentSortFunc = sortFunc;
            HeldCards.Sort((ui1, ui2) => sortFunc(ui1.Data, ui2.Data));
            for (int i = 0; i < HeldCards.Count; i++)
            {
                if (HeldCards[i] == null) continue; // May have been destroyed
                // Move to the correct slot
                HeldCards[i].transform.SetParent(HeldSlots[i].transform);
                HeldCards[i].AnimateToParent();
                HeldSlots[i].SetActive(true);
            }

            // Remove unused slots
            for (int i = HeldCards.Count; i < HeldSlots.Count; i++)
            {
                HeldSlots[i].SetActive(false);
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
                        heldCard.transform.localPosition = Vector3.zero;// Vector3.left * 400;
                        break;
                    case UICardListScreen.ListViewMode.Pillars:
                        heldCard.transform.localPosition = Vector3.up * 1500;
                        break;
                }

                heldCard.transform.localEulerAnglesTransform(new Vector3(0, 0f, 0f), 0f); // fake transition to trigger the delay correctly
                heldCard.transform.localPositionTransition(heldCard.transform.localPosition, 0f); // fake transition to trigger the delay correctly
                var delay = (HeldCards.Count - 1 - iCard + 2) * 0.1f;
                heldCard.transform.localPositionTransition(heldCard.transform.localPosition, delay).JoinTransition().localPositionTransition(Vector3.zero, 0.5f);

                if (!UICardListScreen.FRONT_VIEW_ONLY)
                {
                    if (currentListViewMode == UICardListScreen.ListViewMode.ToDo)
                    {
                        HeldSlots[iCard].transform.localEulerAnglesTransform(new Vector3(0, 0f, -5f), 0.5f);
                    }
                }
            }
        }
    }
}
