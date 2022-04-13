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
            if (def.CategoryDefinition != null)
            {
                Category.Key = def.CategoryDefinition.Title.Key;
            }
            else
            {
                Category.text = string.Empty;
            }
            if (def.SubCategoryDefinition != null)
            {
                Subcategory.Key = def.SubCategoryDefinition.Title.Key;
            }
            else
            {
                Subcategory.text = string.Empty;
            }
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
                StampGO.GetComponent<Animation>().Play("stamp_pulse");
                StampIcon.text = Regex.Unescape("\uf071");
            }
            else if(hasExpirationDate && data.IsDueToday)
            {
                StampGO.SetActive(true);
                StampGO.GetComponent<Animation>().Play("stamp_pulse");
                StampIcon.text = Regex.Unescape("\uf017");
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
            var period = 0.1f;
            rt.localRotationTransition(Quaternion.identity, period);
            rt.anchoredPositionTransition(Vector3.zero, period);
            rt.localPositionTransition_z(0, period);
            rt.localScaleTransition(Vector3.one, period);
        }
    }
}
