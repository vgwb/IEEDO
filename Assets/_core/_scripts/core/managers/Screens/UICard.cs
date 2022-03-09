using System;
using System.Text.RegularExpressions;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;

namespace Ieedo
{
    public class UICard : MonoBehaviour
    {
        public UIText Title;
        public UIText Description;
        public UIText Category;
        public UIText Subcategory;
        public UIText Icon;
        public UIIconsBar Difficulty;
        public UIText Date;

        public GameObject StampGO;
        public UIText StampIcon;

        public Button InteractionButton;

        public Image ColorBase;
        public Image[] BorderImages;
        private CardData data;
        public CardData Data => data;

        public void RefreshUI()
        {
            AssignCard(data);
        }

        public void AssignCard(CardData data)
        {
            this.data = data;
            var def = data.Definition;
            Category.text = def.Category.ToString();
            Subcategory.text = def.SubCategory.ToString();
            Description.text = def.Description.Text;
            Icon.text = Regex.Unescape(def.Icon);
            Title.text = def.Title.Text;
            Category.gameObject.SetActive(def.CategoryDefinition != null);
            Subcategory.gameObject.SetActive(def.CategoryDefinition != null);
            var color = def.CategoryDefinition ? def.CategoryDefinition.Color : new Color(0.8f, 0.8f, 0.8f, 1f);
            ColorBase.color = color;
            Difficulty.gameObject.SetActive(def.Difficulty > 0);
            Difficulty.SetValue(def.Difficulty);

            bool hasExpirationDate = data.ExpirationTimestamp.binaryTimestamp != Timestamp.None.binaryTimestamp;
            Date.gameObject.SetActive(hasExpirationDate);
            Date.text = data.ExpirationTimestamp.ToString();

            foreach (var borderImage in BorderImages)
            {
                borderImage.color = color * 1.4f;
            }

            StampIcon.color = color;
            if (data.Status == CardValidationStatus.Validated)
            {
                StampGO.SetActive(true);
                StampIcon.text = Regex.Unescape("\uf560");
            }
            else if (data.Status == CardValidationStatus.Completed)
            {
                StampGO.SetActive(true);
                StampIcon.text = Regex.Unescape("\uf00c");
            }
            else if (hasExpirationDate && data.IsExpired)
            {
                StampGO.SetActive(true);
                StampGO.GetComponent<Animation>().Play("pulsing");
                StampIcon.text = Regex.Unescape("\uf54c");
            }
            else if(hasExpirationDate && data.IsDueToday)
            {
                StampGO.SetActive(true);
                StampGO.GetComponent<Animation>().Play("pulsing");
                StampIcon.text = Regex.Unescape("\uf06a");
            }
            else
            {
                StampGO.SetActive(false);
            }
        }

        public void OnInteraction(Action action)
        {
            InteractionButton.onClick.RemoveAllListeners();
            InteractionButton.onClick.AddListener(() => action?.Invoke());
        }

        public void AnimateToParent()
        {
            var rt = transform as RectTransform;
            rt.rotationTransition(Quaternion.identity, 0.25f);
            rt.anchoredPositionTransition(Vector3.zero, 0.25f);
            rt.localScaleTransition(Vector3.one, 0.25f);
        }
    }
}
