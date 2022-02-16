using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Ieedo
{
    public class UIAssessmentIntroScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.AssessmentIntro;

        public UITextContent Title;

        public RectTransform CategoriesPivot;

        public UIButton ContinueButton;

        public IEnumerator ShowIntro()
        {
            Title.Text.text = "Intro to assessment";

            var categoryTexts = CategoriesPivot.GetComponentsInChildren<UITextContent>(true);

            var categories = Statics.Data.GetAll<CategoryDefinition>();
            for (var iCategory = 0; iCategory < categories.Count; iCategory++)
            {
                var category = categories[iCategory];
                categoryTexts[iCategory].Text.text = category.Title.Text;
                categoryTexts[iCategory].BG.color = category.Color;
            }

            SetupButton(ContinueButton, Close);
            yield return OpenCO();
        }

    }
}