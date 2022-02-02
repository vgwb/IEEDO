using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ieedo.Test
{
    public class PillarView : MonoBehaviour
    {
        public GameObject cardPrefab;
        public List<GameObject> cards = new List<GameObject>();

        public Transform gfx;
        public MeshRenderer mr;
        public TextMeshPro text;

        public void ShowData(PillarData data)
        {
            var baseScale = 0.1f;
            gfx.localScale = new Vector3(1f, data.Height, 1f)*baseScale;
            mr.material = new Material(mr.material);
            mr.material.SetColor("_Color", data.Color);
            mr.material.SetColor("_EmissionColor", data.Color*0.5f);
            text.text = $"{data.Height * 100}%";

            foreach (var card in cards)
                card.SetActive(false);

            for (int iCard = 0; iCard < data.NCards; iCard++)
            {
                if (iCard >= cards.Count)
                {
                    var cardGo = Instantiate(cardPrefab, transform);
                    cards.Add(cardGo);
                    cardGo.transform.localPosition = Vector3.up * (data.Height + iCard * 0.025f);
                    cardGo.transform.localPosition += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
                    cardGo.transform.localEulerAngles = Vector3.up * Random.Range(0, 360f);
                    cardGo.GetComponentInChildren<MeshRenderer>().material = new Material(cardGo.GetComponentInChildren<MeshRenderer>().material);
                    cardGo.GetComponentInChildren<MeshRenderer>().material.color = data.Color;
                }
                cards[iCard].SetActive(true);
            }
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