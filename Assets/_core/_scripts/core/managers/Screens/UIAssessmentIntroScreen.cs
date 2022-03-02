using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization;

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
            Title.Text.Key = new LocalizedString("UI", "assessment_intro_title");

            var categoryTexts = CategoriesPivot.GetComponentsInChildren<UITextContent>(true);

            var categories = Statics.Data.GetAll<CategoryDefinition>();
            for (var iCategory = 0; iCategory < categories.Count; iCategory++)
            {
                var category = categories[iCategory];
                categoryTexts[iCategory].Text.Key = category.Title.Key;
                categoryTexts[iCategory].BG.color = category.Color;
            }

            SetupButton(ContinueButton, Close);
            yield return OpenCO();
        }

    }
}
