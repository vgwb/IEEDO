using System;
using System.Collections;
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

        public Action OnDragStart;
        public Action OnDragEnd;
        public Action OnCardSelected;
        public Action OnCardInFront;
        public Action OnCardNotInFront;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            hasStoppedPhysicalInertia = false;
            base.OnBeginDrag(eventData);
            OnDragStart?.Invoke();
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            base.OnEndDrag(eventData);
            OnDragEnd?.Invoke();
        }

        protected override void OnEnable()
        {
            //normalizedPosition = Vector2.one;
            hasInFront = false;
            base.OnEnable();
        }


        public void ForceToPos(float normalizedPos)
        {
            normalizedPosition = new Vector3(normalizedPos, 1);
            Debug.LogError("FORCE NORMALIZED POS: " + normalizedPosition);
        }

        public float StopInertiaThreshold = 5f;
        public float SnappingSpeed = 1f;

        private int prevCardIndex;
        private bool hasInFront;
        private Vector2 prevVelocity;
        private int forcedCardIndex;
        private bool forceGoToCard;

        public IEnumerator ForceGoToCard(UICard card)
        {
            forceGoToCard = true;
            forcedCardIndex = CardCollection.HeldCards.IndexOf(card);
            while (forceGoToCard) yield return null;
            hasInFront = true; // Force this, otherwise it will trigger again later
        }

        public bool CanScroll = true;
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

            var wantedCardIndex = centerCardIndex;
            if (forceGoToCard) wantedCardIndex = forcedCardIndex;

            //var layout = GetComponentInChildren<HorizontalLayoutGroup>(true);
            //var padding = layout.padding.left + layout.padding.right;
            //var size = layout.GetComponent<RectTransform>().rect.width;
            //var contentPercent = (size - padding) / size;
            //Debug.LogError("CONTENT PERCENT: " + contentPercent);

            var desiredNormalizedPos = wantedCardIndex * 1f / (Mathf.Max(1f, nCards - 1));
            //desiredNormalizedPos /= contentPercent;
            var diff = (desiredNormalizedPos - normalizedPosition.x);

            var normalizedStep = 1f / Mathf.Max(1,(nCards -1)*2);
            // WIth 2, it works with 2.
            // with 3 cards, it works with 4.
            // With 4 cards, nroamlized is 0.25, but should be 0.1666 (1/6)
            // With 5 cards, nroamlized is 0.2, but should be 0.125 (1/8)
            //Debug.LogError("CEnter card index " + centerCardIndex);
            //Debug.LogError("desiredNormalizedPos " + desiredNormalizedPos);
            //Debug.LogError("Diff " + diff);
            //Debug.LogError("Normalized step " + normalizedStep);
            float ratio = Mathf.Abs(diff) / normalizedStep;
            if (nCards == 1) ratio = 0f;
            var scaleFactor = CardCollection.CardScale * (0.3f * (1 - ratio));

            scaleFactor = Mathf.Max(0, scaleFactor);
            //Debug.LogError("Scale factor " + scaleFactor);

            CardCollection.HeldSlots[centerCardIndex].transform.localScale = Vector3.one * (baseScale + scaleFactor);

            CardListScreen.ForceFrontCard(CardCollection.HeldCards[centerCardIndex]);

            var accel = (velocity.x - prevVelocity.x) / Time.deltaTime;
            //Debug.Log($"ACC <color={(velocity.x > 0 ? "#00FF00" : "#FF0000")}>{velocity.x}</color> ACC <color={(accel > 0 ? "#00FF00" : "#FF0000")}>{accel}</color>");

            prevVelocity = velocity;

            // While NOT dragging, change velocity to snap
            if (!hasStoppedPhysicalInertia)
            {
                if (Mathf.Abs(velocity.x) < StopInertiaThreshold && Math.Abs(Mathf.Sign(accel) - Mathf.Sign(velocity.x)) > 0.01f)
                {
                    //Debug.LogError("STOP PHYSICAL INERTIA");
                    hasStoppedPhysicalInertia = true;
                }
                else
                {
                    return;
                }
            }

            // We perform snapping from this point
            var ratioThreshold = 0.3f;
            if (!isDragging)
            {
                v = new Vector2(- Mathf.Sign(diff) * Mathf.Abs(diff) * Mathf.Abs(diff) * SnappingSpeed * nCards * 100000, 0f);
                //Debug.LogError("DESIRED VELOCITY " + v  + "(" + normalizedPosition.x + " to " + desiredNormalizedPos +")");
                velocity = v;

                //Debug.LogError("ratio " + ratio);
                if (ratio < ratioThreshold && !hasInFront)
                {
                    hasInFront = true;
                    OnCardInFront?.Invoke();
                    //Debug.LogError("IN FRONT");
                    forceGoToCard = false;
                }
            }
            if (ratio > ratioThreshold && hasInFront)
            {
                hasInFront = false;
                OnCardNotInFront?.Invoke();
                //Debug.LogError("NOT FRONT");
            }

            // When switching, we set the new card
            if (centerCardIndex != prevCardIndex)
            {
                OnCardSelected?.Invoke();
                prevCardIndex = centerCardIndex;
            }
        }
    }
}
