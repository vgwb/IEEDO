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

        public void ShowLabel(bool choice)
        {
            if (choice)
            {
                labelText.gameObject.SetActive(true);
                labelText.GetComponent<LocalizeStringEvent>().StringReference = data.LocalizedKey;
                var targetColor = labelText.color;
                labelText.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
                labelText.colorTransition(targetColor, 0.5f);
            }
            else
            {
                labelText.gameObject.SetActive(false);
            }
        }

        private int nCurrentCards;
        public void ShowData(PillarData data, bool added)
        {
            if (!added) nCurrentCards = 0;

            this.data = data;
            var baseScale = 0.1f;

            if (!added)
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
                if (iCard >= cardGos.Count)
                {
                    AddNewCard(iCard);
                }

                var cardGo = cardGos[iCard].gameObject;
                var mr = cardGo.GetComponentInChildren<MeshRenderer>();
                mr.material.color = data.Cards[iCard].Definition.CategoryDefinition.Color * (1.4f + Random.Range(-0.2f, 0.2f));

                cardGos[iCard].SetActive(true);
                if (nCurrentCards <= iCard) nCurrentCards++;
            }
            for (int iCard = data.NCards; iCard < cardGos.Count; iCard++)
            {
                cardGos[iCard].SetActive(false);
            }

            //Debug.Log("Showing pillar " + data.LocalizedKey.GetLocalizedString() + " with " + nCurrentCards + " (previous " + nPreviousCards + ")");

            int startNewIndex = nPreviousCards;
            CardsIn(startNewIndex);
        }

        private Vector3 ComputeFinalPos(int iCard)
        {
            float pillarTop = gfxHeight * 2.5f;
            var finalPos = Vector3.up * (pillarTop + (0.5f+iCard) * 0.05f);
            finalPos += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
            return finalPos;
        }

        public void CardsIn(int fromCardIndex = 0)
        {
            for (int iCard = fromCardIndex; iCard < data.NCards; iCard++)
            {
                var cardGo = cardGos[iCard].gameObject;
                cardGo.transform.localPosition = Vector3.up * 15;
                cardGo.transform.localEulerAngles = Vector3.zero;

                var finalPos = ComputeFinalPos(iCard);
                var period = 1f;
                cardGo.transform.localPositionTransition(cardGo.transform.localPosition, 0.0f); // Fake transition to make the delay work correctly
                cardGo.transform.JoinDelayTransition((iCard-fromCardIndex) * 0.1f).localPositionTransition(finalPos, period, LeanEase.Bounce)
                    .localEulerAnglesTransform(Vector3.up * Random.Range(0, 360f), period);
            }
        }

        public void CardsOut()
        {
            for (int iCard = 0; iCard < data.NCards; iCard++)
            {
                var cardGo = cardGos[iCard].gameObject;
                float period = 0.25f;
                cardGo.transform.localPositionTransition_y(15, period, LeanEase.Smooth);
                cardGo.transform.positionTransition_x(2f, period, LeanEase.Smooth);
                cardGo.transform.localRotationTransition(Quaternion.Euler(Random.Range(0, 360f),  Random.Range(0, 360f),  Random.Range(0, 360f)), period);
            }
        }

        public void AddNewCard(int iCard)
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
