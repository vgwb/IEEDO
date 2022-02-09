using System;
using System.Text.RegularExpressions;
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

        public void AssignCard(CardData data)
        {
            this.data = data;
            var def = data.Definition;
            Category.text = def.Category.ToString();
            Subcategory.text = def.SubCategory.ToString();
            Description.text = def.Description.Text;
            Icon.text = Regex.Unescape(def.Icon);
            Title.text = def.Title.Text;
            ColorBase.color = def.CategoryDefinition.Color;
            Difficulty.SetValue(def.Difficulty);

            Date.text = data.ExpirationTimestamp.ToString();

            foreach (var borderImage in BorderImages)
            {
                borderImage.color = def.CategoryDefinition.Color * 1.4f;
            }

            if (data.IsExpired)
            {
                StampGO.SetActive(true);
                StampIcon.text = Regex.Unescape("\uf54c");
            }
            else if(data.IsDueToday)
            {
                StampGO.SetActive(true);
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
    }
}