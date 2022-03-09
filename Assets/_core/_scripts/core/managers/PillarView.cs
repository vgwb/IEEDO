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
        public List<GameObject> cards = new List<GameObject>();

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

        public void ShowData(PillarData data)
        {
            this.data = data;
            var baseScale = 0.1f;
            gfx.localScale = new Vector3(1f, gfxHeight, 1f)*baseScale;
            mr.material = new Material(mr.material);
            mr.material.SetColor("_Color", data.Color);
            mr.material.SetColor("_EmissionColor", data.Color*0.5f);
            ShowLabel(false);
            ShowValue(true);

            foreach (var card in cards)
                card.SetActive(false);

            for (int iCard = 0; iCard < data.NCards; iCard++)
            {
                if (iCard >= cards.Count)
                {
                    AddNewCard(iCard);
                }

                var cardGo = cards[iCard].gameObject;

                cardGo.transform.localPosition = Vector3.up * 25;
                cardGo.transform.localEulerAngles = Vector3.zero;

                var mr = cardGo.GetComponentInChildren<MeshRenderer>();
                mr.material.color = data.Color;

                cards[iCard].SetActive(true);
            }

            CardsIn();
        }

        private Vector3 ComputeFinalPos(int iCard)
        {
            float pillarTop = gfxHeight * 2.5f;
            var finalPos = Vector3.up * (pillarTop + iCard * 0.025f);
            finalPos += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
            return finalPos;
        }

        public void CardsIn()
        {
            for (int iCard = 0; iCard < data.NCards; iCard++)
            {
                var cardGo = cards[iCard].gameObject;
                var finalPos = ComputeFinalPos(iCard);
                cardGo.transform.localPositionTransition(finalPos, 1f, LeanEase.Bounce);
                cardGo.transform.localEulerAnglesTransform(Vector3.up * Random.Range(0, 360f), 1f);
            }
        }

        public void CardsOut()
        {
            for (int iCard = 0; iCard < data.NCards; iCard++)
            {
                var cardGo = cards[iCard].gameObject;
                cardGo.transform.localPositionTransition(Vector3.up*10, 0.25f, LeanEase.Smooth);
                cardGo.transform.localEulerAnglesTransform(Vector3.up * Random.Range(0, 360f), 1f);
            }
        }

        public void AddNewCard(int iCard)
        {
            var cardGo = Instantiate(cardPrefab, transform);
            cardGo.GetComponentInChildren<MeshRenderer>().material = new Material(cardGo.GetComponentInChildren<MeshRenderer>().material);
            cards.Add(cardGo);
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
