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
        private bool hasTriggeredSwipe;
        private float swipeStartPos;
        private int swipeFromCardIndex;
        private int swipeToCardIndex;

        public Action OnDragStart;
        public Action OnDragEnd;
        public Action OnCardSelected;
        public Action OnCardInFront;
        public Action OnCardNotInFront;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            hasInFront = false;
            swipeStartPos = content.anchoredPosition.x;

            var nCards = CardCollection.HeldCards.Count;
            float highestScaleRatio = 0f;
            int highestScaleCardIndex = 0;
            if (uiCamera == null)
                uiCamera = GameObject.Find("CameraUI").GetComponent<Camera>();
            for (var index = 0; index < CardCollection.HeldCards.Count; index++)
            {
                var card = CardCollection.HeldCards[index];
                var screenPos = uiCamera.WorldToScreenPoint(card.transform.position);
                var normalizedScreenPos = screenPos.x / Screen.width;

                var scaleRatio = 1 - Mathf.Abs(normalizedScreenPos - 0.5f) * 2f;
                if (nCards == 1)
                    scaleRatio = 1f;

                if (scaleRatio > highestScaleRatio)
                {
                    highestScaleRatio = scaleRatio;
                    highestScaleCardIndex = index;
                }
            }

            swipeFromCardIndex = highestScaleCardIndex;

            base.OnBeginDrag(eventData);
            OnDragStart?.Invoke();
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            hasTriggeredSwipe = true;
            //swipeStartPos = content.anchoredPosition.x;
            swipeToCardIndex = -1;

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
            //Debug.LogError("FORCE NORMALIZED POS: " + normalizedPosition);
        }

        public float SnappingSpeed = 1f;

        private int prevCardIndex;
        private bool hasInFront;
        private Vector2 prevVelocity;
        private int forcedCardIndex;
        private bool forceGoToCard;
        private bool forceGoToCardImmediate;

        public IEnumerator ForceGoToCard(UICard card, bool immediate = false)
        {
            //Debug.LogError("FORCE GO TO CARD! Immediate? " + immediate);
            forceGoToCard = true;
            forceGoToCardImmediate = immediate;
            forcedCardIndex = CardCollection.HeldCards.IndexOf(card);
            while (forceGoToCard)
                yield return null;
            enabled = true; // Reset enabled now as we reached the correct card
            hasInFront = true; // Force this, otherwise it will trigger again later
        }

        private Camera uiCamera;
        public bool CanScroll = true;
        public void Update()
        {
            if (CardListScreen == null)
                CardListScreen = FindObjectOfType<UICardListScreen>();
            if (!Application.isPlaying)
                return;

            //Debug.Log(content.anchoredPosition.x);

            // When not scrolling, snap to a card and set it as the FRONT one
            var v = velocity;
            //Debug.LogError("Velocity " + v + " Position " + normalizedPosition);

            var nCards = CardCollection.HeldCards.Count;
            if (nCards == 0)
                return;

            var baseScale = CardCollection.CardScale * 0.8f;
            foreach (var slot in CardCollection.HeldSlots)
            {
                slot.transform.localScale = Vector3.one * baseScale;
            }

            float highestScaleRatio = 0f;
            int highestScaleCardIndex = 0;
            if (uiCamera == null)
                uiCamera = GameObject.Find("CameraUI").GetComponent<Camera>();
            highestScaleCardIndex = ResizeAndFindHighestScaleCardIndex();

            int ResizeAndFindHighestScaleCardIndex()
            {
                for (var index = 0; index < CardCollection.HeldCards.Count; index++)
                {
                    var card = CardCollection.HeldCards[index];
                    var screenPos = uiCamera.WorldToScreenPoint(card.transform.position);
                    var normalizedScreenPos = screenPos.x / Screen.width;
                    //card.Title.SetTextRaw(normalizedScreenPos.ToString("F"));
                    //if (index == centerCardIndex) card.Title.SetTextRaw("CENTER " + normalizedScreenPos);

                    var scaleRatio = 1 - Mathf.Abs(normalizedScreenPos - 0.5f) * 2f;
                    if (nCards == 1)
                        scaleRatio = 1f;
                    var scaleFactor = CardCollection.CardScale * (0.3f * (scaleRatio));
                    scaleFactor = Mathf.Max(0, scaleFactor);
                    CardCollection.HeldSlots[index].transform.localScale = Vector3.one * (baseScale + scaleFactor);

                    if (scaleRatio > highestScaleRatio)
                    {
                        highestScaleRatio = scaleRatio;
                        highestScaleCardIndex = index;
                    }
                }

                return highestScaleCardIndex;
            }

            var centerCardIndex = highestScaleCardIndex;
            centerCardIndex = Mathf.Clamp(centerCardIndex, 0, nCards - 1);

            var wantedCardIndex = centerCardIndex;
            if (swipeToCardIndex >= 0)
                wantedCardIndex = swipeToCardIndex;
            if (forceGoToCard)
            {
                swipeToCardIndex = -1; // Override this, forcing has precedence
                wantedCardIndex = forcedCardIndex;
            }

            var layout = GetComponentInChildren<HorizontalLayoutGroup>(true);
            var spacing = layout.spacing;
            var size = layout.GetComponent<RectTransform>().rect.width;

            var slotSize = CardCollection.HeldSlots[0].GetComponent<RectTransform>().rect.width;
            var desiredPos = -(layout.padding.left + wantedCardIndex * slotSize + (0.5f * slotSize) + wantedCardIndex * spacing);
            var actualPos = content.anchoredPosition.x;

            if (hasTriggeredSwipe && swipeToCardIndex < 0 && Mathf.Abs(actualPos - swipeStartPos) > 0.1f)
            {
                var direction = -(int)Mathf.Sign(actualPos - swipeStartPos);
                //Debug.LogError("actualPos  " + actualPos + " swipeStartPos " + swipeStartPos);
                //Debug.LogError("direction  " + direction);
                //Debug.LogError("highestScaleCardIndex " + highestScaleCardIndex);

                // Make sure that the wanted card is always the next one in the direction of movement
                wantedCardIndex = swipeFromCardIndex + direction;
                wantedCardIndex = Mathf.Clamp(wantedCardIndex, 0, nCards - 1);

                // Recompute the desired position towards the next one instead
                desiredPos = -(layout.padding.left + wantedCardIndex * slotSize + (0.5f * slotSize) + wantedCardIndex * spacing);

                swipeToCardIndex = wantedCardIndex;
                hasTriggeredSwipe = false;
                //Debug.LogError("SWIPING TOWARDS  " + wantedCardIndex + " from card " + swipeFromCardIndex + " center is " + centerCardIndex);
            }

            //Debug.LogError("Pos of wanted: " + desiredPos);
            //var desiredNormalizedPos = desiredPos / size;

            //Debug.LogError("size  " + size + " padding " + padding);
            //Debug.LogError("DESIRED NORMALIZED  " + desiredNormalizedPos);
            //Debug.LogError("ACTUAL NORMALIZED  " + normalizedPosition.x);
            //Debug.LogError("DESIRED POS  " + desiredPos);
            //Debug.LogError("ACTUAL POS  " + actualPos);

            var normalizedDiff = (desiredPos - actualPos) / size;

            var normalizedStep = 1f / Mathf.Max(1, (nCards - 1) * 2);
            // With 2, it works with 2.
            // With 3 cards, it works with 4.
            // With 4 cards, normalized is 0.25, but should be 0.1666 (1/6)
            // With 5 cards, normalized is 0.2, but should be 0.125 (1/8)
            //Debug.LogError("CEnter card index " + centerCardIndex);
            //Debug.LogError("desiredNormalizedPos " + desiredNormalizedPos);
            //Debug.LogError("Diff " + diff);
            //Debug.LogError("Normalized step " + normalizedStep);

            float ratio = Mathf.Abs(normalizedDiff) / normalizedStep;

            CardListScreen.ForceFrontCard(CardCollection.HeldCards[centerCardIndex]);

            //var accel = (velocity.x - prevVelocity.x) / Time.deltaTime;
            //Debug.Log($"ACC <color={(velocity.x > 0 ? "#00FF00" : "#FF0000")}>{velocity.x}</color> ACC <color={(accel > 0 ? "#00FF00" : "#FF0000")}>{accel}</color>");

            prevVelocity = velocity;

            // We perform snapping from this point
            var ratioThreshold = 0.3f;
            if (!isDragging)    // @note: we snap only if the card has changed from the previous one
            {
                v = new Vector2(Mathf.Sign(normalizedDiff) * Mathf.Abs(normalizedDiff) * Mathf.Abs(normalizedDiff) * SnappingSpeed * nCards * 100000, 0f);
                //Debug.LogError("DESIRED VELOCITY " + v  + "(" + normalizedPosition.x + " to " + desiredNormalizedPos +")");
                //if (forceGoToCard || swipeToCardIndex >= 0)
                velocity = v;

                //Debug.LogError("ratio " + ratio);
                if (forceGoToCardImmediate || (ratio < ratioThreshold && (!hasInFront || forceGoToCard) && centerCardIndex == wantedCardIndex))
                {
                    // When we have it in front, if forcing, SNAP IT, to make sure it arrives at the correct position
                    if (forceGoToCard)
                    {
                        content.anchoredPosition = new Vector2(desiredPos, content.anchoredPosition.y);
                        //Debug.LogError("FORCED FINAL POSITION TO " + desiredPos);
                        velocity = Vector2.zero;// = false; // @note: we always need to disable the scrolling when forcing the movement, or it will mess up the position
                        prevCardIndex = centerCardIndex;
                        CardListScreen.ForceFrontCard(CardCollection.HeldCards[wantedCardIndex]);
                        ResizeAndFindHighestScaleCardIndex();
                    }

                    hasInFront = true;
                    forceGoToCardImmediate = false;
                    OnCardInFront?.Invoke();
                    //Debug.LogError("IN FRONT " + wantedCardIndex + " (center is " + centerCardIndex + ")");
                    forceGoToCard = false;
                    swipeToCardIndex = -1;
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
                //Debug.LogError("OnCardSelected");
            }
        }
    }
}
