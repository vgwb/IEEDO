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

        public Button InteractionButton;

        public Image ColorBase;
        private CardDefinition def;

        public void AssignDefinition(CardDefinition def)
        {
            Debug.LogError("Load card " + def);
            this.def = def;
            Category.text = def.Category.ToString();
            //uiCard.Subcategory.text = card.Subcategory.ToString();
            Description.text = def.Description.Text;
            Icon.text = Regex.Unescape(def.Icon);
            Title.text = def.Title.Text;
            ColorBase.color = Statics.Data.Get<CategoryDefinition>((int)def.Category).Color.Color;
        }
    }
}