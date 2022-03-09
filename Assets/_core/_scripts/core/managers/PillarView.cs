using System;
using System.Collections.Generic;
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
        public TextMeshPro text;

        void Awake()
        {
            var selectable = GetComponentInChildren<LeanSelectableByFinger>();
            selectable.OnSelected.AddListener(() => OnSelected?.Invoke());
        }

        private PillarData data;
        public PillarData Data => data;
        private float gfxHeight => Mathf.Max(data.Height, 0.05f);

        public void ShowHeight()
        {
            text.text = $"{Mathf.RoundToInt(data.Height*100)}%";
        }

        public void ShowLabel()
        {
            text.GetComponent<LocalizeStringEvent>().StringReference = data.LocalizedKey;
        }

        public void ShowData(PillarData data)
        {
            this.data = data;
            var baseScale = 0.1f;
            gfx.localScale = new Vector3(1f, gfxHeight, 1f)*baseScale;
            mr.material = new Material(mr.material);
            mr.material.SetColor("_Color", data.Color);
            mr.material.SetColor("_EmissionColor", data.Color*0.5f);
            if (data.PrioritizeLabel) ShowLabel();
            else ShowHeight();

            foreach (var card in cards)
                card.SetActive(false);

            for (int iCard = 0; iCard < data.NCards; iCard++)
            {
                if (iCard >= cards.Count)
                {
                    AddNewCard(iCard);
                }

                var cardGo = cards[iCard].gameObject;

                float pillarTop = gfxHeight * 2.5f;
                cardGo.transform.localPosition = Vector3.up * 25;
                cardGo.transform.localEulerAngles = Vector3.zero;

                var finalPos = Vector3.up * (pillarTop + iCard * 0.025f);
                finalPos += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));

                cardGo.transform.localPositionTransition(finalPos, 1f, LeanEase.Bounce);
                cardGo.transform.localEulerAnglesTransform(Vector3.up * Random.Range(0, 360f), 1f);

                var mr = cardGo.GetComponentInChildren<MeshRenderer>();
                mr.material.color = data.Color;

                cards[iCard].SetActive(true);
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
