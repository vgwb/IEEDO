using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{
    public class UIAssessmentRecapPopup : UIScreen
    {
        public override ScreenID ID => ScreenID.AssessmentRecap;

        public UITextContent Title;

        public UIAssessmentResultLine[] Categories;
        public UIAssessmentResultLine Overall;

        public UIButton ContinueButton;

        public void ShowResults(Dictionary<int, float> assessmentPercentages, float overallValue)
        {
            var categories = Statics.Data.GetAll<CategoryDefinition>();
            for (var iCategory = 0; iCategory < categories.Count; iCategory++)
            {
                var category = categories[iCategory];

                Categories[iCategory].Title.SetText(category.Title.Text);
                Categories[iCategory].Value.SetValue(Mathf.RoundToInt(assessmentPercentages[category.Id]*100), 100);
                Categories[iCategory].BG.color = category.Color;
                Categories[iCategory].Value.FillImage.color = category.Color.SetValue(1f);
                Categories[iCategory].Value.BGImage.color = category.Color.SetValue(0.3f);
            }

            Overall.Title.text = "Overall";
            Overall.Value.SetValue(Mathf.RoundToInt(overallValue*100), 100);
            Overall.BG.color = Color.black;
            Overall.Value.FillImage.color = Color.black.SetValue(1f);
            Overall.Value.BGImage.color = Color.black.SetValue(0.3f);

            OpenImmediate();

            SetupButton(ContinueButton, CloseImmediate);
        }
    }
}