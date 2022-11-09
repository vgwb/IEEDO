using Lean.Transition;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ieedo
{
    public class SnappingScrollRect : ScrollRect
    {
        public UICardCollection CardCollection;
        public UICardListScreen CardListScreen;

        private bool isDragging;
        private bool hasStoppedPhysicalInertia;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            hasStoppedPhysicalInertia = false;
            base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            base.OnEndDrag(eventData);
        }

        public void Update()
        {
            if (CardListScreen == null) CardListScreen = FindObjectOfType<UICardListScreen>();

            if (!Application.isPlaying) return;

            // When not scrolling, snap to a card and set it as the FRONT one
            var v = velocity;
            //Debug.LogError("Velocity " + v + " Position " + normalizedPosition);

            var nCards = CardCollection.HeldCards.Count;
            if (nCards == 0) return;

            var centerCardIndex = nCards == 1 ? 0 : Mathf.RoundToInt(normalizedPosition.x * (nCards-1));
            centerCardIndex = Mathf.Clamp(centerCardIndex, 0, nCards - 1);

            var baseScale = CardCollection.CardScale * 0.8f;
            foreach (var slot in CardCollection.HeldSlots)
            {
                slot.transform.localScale = Vector3.one * baseScale;
            }
            var desiredNormalizedPos = centerCardIndex * 1f / (nCards - 1);
            var diff = (desiredNormalizedPos - normalizedPosition.x);

            var normalizedStep = 1f / (nCards + 1);
            CardCollection.HeldSlots[centerCardIndex].transform.localScale = Vector3.one * (baseScale + CardCollection.CardScale * (0.3f * (1 - Mathf.Abs(diff) / normalizedStep)));

            CardListScreen.ForceFrontCard(CardCollection.HeldCards[centerCardIndex]);

            // While NOT dragging, change velocity to snap
            if (!hasStoppedPhysicalInertia)
            {
                if (Mathf.Abs(velocity.x) < 0.05f)
                {
                    hasStoppedPhysicalInertia = true;
                }
                else
                {
                    return;
                }
            }

            if (!isDragging)
            {
                v = new Vector2(- Mathf.Sign(diff) * Mathf.Abs(diff) * Mathf.Abs(diff) * 50000, 0f);
                //Debug.LogError("DESIRED VELOCITY " + v  + "(" + normalizedPosition.x + " to " + desiredNormalizedPos +")");
                velocity = v;
            }
        }
    }
}
