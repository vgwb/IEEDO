using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lean.Common;
using Lean.Touch;
using Lean.Transition;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using Random = UnityEngine.Random;

namespace Ieedo
{
    public class PillarView : MonoBehaviour
    {
        public Action OnSelected;

        public GameObject cardPrefab;
        public List<GameObject> cardGos = new List<GameObject>();

        public Transform gfx;
        public MeshRenderer mr;
        public TextMeshPro valueText;
        public TextMeshPro labelText;

        void Awake()
        {
            var selectable = GetComponentInChildren<LeanSelectableByFinger>();
            selectable.OnSelected.AddListener(() => OnSelected?.Invoke());
            labelColor = labelText.color;
        }

        private PillarData data;
        public PillarData Data => data;
        private float gfxHeight => Mathf.Max(data.Height, 0.05f);

        public void ShowValue(bool choice)
        {
            if (choice)
            {
                valueText.gameObject.SetActive(true);
                if (!data.IconString.IsNullOrEmpty())
                    valueText.text = Regex.Unescape(data.IconString);
                else
                    valueText.text = $"{Mathf.RoundToInt(data.Height*100)}%";
            }
            else valueText.gameObject.SetActive(false);
        }

        private bool isShowingLabel = false;
        private Color labelColor;
        public void ShowLabel(bool choice)
        {
            if (choice && !isShowingLabel)
            {
                isShowingLabel = true;
                labelText.colorTransition(labelColor, 1f);
            }
            else if (!choice && isShowingLabel)
            {
                isShowingLabel = false;
                labelText.colorTransition(new Color(labelColor.r, labelColor.g, labelColor.b, 0f), 1f);
            }
        }

        private int nCurrentCards;
        public void ShowData(PillarData data, bool showOnlyNewlyAddedCards)
        {
            this.data = data;

            if (!showOnlyNewlyAddedCards) nCurrentCards = 0;
            else nCurrentCards = Mathf.Min(data.Cards.Count, nCurrentCards);

            labelText.GetComponent<LocalizeStringEvent>().StringReference = data.LocalizedKey;
            labelText.color = new Color(labelColor.r, labelColor.g, labelColor.b, 0f);
            var baseScale = 0.1f;

            if (!showOnlyNewlyAddedCards)
            {
                gfx.localScale = new Vector3(1f, 0f, 1f)*baseScale;
                gfx.localScaleTransition_y(gfxHeight * baseScale, 0.25f);
            }
            else
            {
                gfx.localScale = new Vector3(1f, gfxHeight, 1f)*baseScale;
            }
            mr.material = new Material(mr.material);
            mr.material.SetColor("_Color", data.Color);
            mr.material.SetColor("_EmissionColor", data.Color*0.5f);
            ShowLabel(false);
            ShowValue(true);

            int nPreviousCards = nCurrentCards;
            for (int iCard = 0; iCard < data.NCards; iCard++)
            {
                AddCard(iCard, data.Cards[iCard]);
            }
            for (int iCard = data.NCards; iCard < cardGos.Count; iCard++)
            {
                cardGos[iCard].SetActive(false);
            }

            //Debug.Log("Showing pillar " + data.LocalizedKey.GetLocalizedString() + " with " + nCurrentCards + " (previous " + nPreviousCards + ")");

            int startNewIndex = nPreviousCards;
            CardsIn(startNewIndex);
        }

        public void RemoveSingleCard(CardData cardData)
        {
            data.Cards.Remove(cardData);
            nCurrentCards--;
        }

        public void AddSingleCard(CardData cardData)
        {
            data.Cards.Add(cardData);
            AddCard(nCurrentCards, cardData);
            CardsIn(nCurrentCards-1);
        }

        public void AddCard(int iCard, CardData cardData)
        {
            if (iCard >= cardGos.Count)
            {
                AddNewCard();
            }

            var cardGo = cardGos[iCard].gameObject;
            var mr = cardGo.GetComponentInChildren<MeshRenderer>();
            mr.material.color = cardData.Definition.CategoryDefinition.Color * (1.4f + Random.Range(-0.2f, 0.2f));
            cardGos[iCard].SetActive(true);
            if (nCurrentCards <= iCard) nCurrentCards++;
        }

        private Vector3 ComputeFinalPos(int iCard)
        {
            float pillarTop = gfxHeight * 2.5f;
            var finalPos = Vector3.up * (pillarTop + (0.5f+iCard) * 0.075f);
            finalPos += new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
            return finalPos;
        }

        public void CardsIn(int fromCardIndex = 0)
        {
            int nAnimatedCards = 0;
            for (int iCard = fromCardIndex; iCard < data.NCards; iCard++)
            {
                var cardGo = cardGos[iCard].gameObject;
                cardGo.transform.localPosition = Vector3.up * 15;
                cardGo.transform.localEulerAngles = Vector3.zero;

                var finalPos = ComputeFinalPos(iCard);
                var period = 0.75f;
                period -= nAnimatedCards * 0.025f;
                period = Mathf.Max(0.025f, period);
                cardGo.transform.localPositionTransition(cardGo.transform.localPosition, 0.0f); // Fake transition to make the delay work correctly
                cardGo.transform.JoinDelayTransition((iCard - fromCardIndex) * 0.1f).localPositionTransition(finalPos, period, LeanEase.Accelerate);
                cardGo.transform.localEulerAnglesTransform(Vector3.up * Random.Range(0, 360f), 1.25f, LeanEase.Decelerate);
                nAnimatedCards++;
            }
        }

        public void CardsOut()
        {
            for (int iCard = 0; iCard < data.NCards; iCard++)
            {
                var cardGo = cardGos[iCard].gameObject;
                float period = 0.25f;
                //cardGo.transform.GetTransition().Stop();
                cardGo.transform.localPositionTransition(cardGo.transform.localPosition, 0.0f);     // This forces a stop
                cardGo.transform.localPositionTransition_y(15, period, LeanEase.Smooth);
                cardGo.transform.positionTransition_x(2f, period, LeanEase.Smooth);
                cardGo.transform.localRotationTransition(Quaternion.Euler(Random.Range(0, 360f),  Random.Range(0, 360f),  Random.Range(0, 360f)), period);
            }
        }

        public void AddNewCard()
        {
            var cardGo = Instantiate(cardPrefab, transform);
            cardGo.GetComponentInChildren<MeshRenderer>().material = new Material(cardGo.GetComponentInChildren<MeshRenderer>().material);
            cardGos.Add(cardGo);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

    }
}
