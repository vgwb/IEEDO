using System;
using System.Text.RegularExpressions;
using Lean.Gui;
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

        public Button InteractionButton;

        public Image ColorBase;
        public Image[] BorderImages;
        private CardDefinition def;
        public CardDefinition Definition => def;

        public void AssignDefinition(CardDefinition def)
        {
            //Debug.LogError("Load card " + def);
            this.def = def;
            Category.text = def.Category.ToString();
            Subcategory.text = def.SubCategory.ToString();
            Description.text = def.Description.Text;
            Icon.text = Regex.Unescape(def.Icon);
            Title.text = def.Title.Text;
            ColorBase.color = def.CategoryDefinition.Color;

            foreach (var borderImage in BorderImages)
            {
                borderImage.color = def.CategoryDefinition.Color * 1.4f;
            }
        }

        public void OnInteraction(Action action)
        {
            InteractionButton.onClick.RemoveAllListeners();
            InteractionButton.onClick.AddListener(() => action?.Invoke());
        }
    }
}