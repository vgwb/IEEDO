using System.Collections;

namespace Ieedo
{
    public class UIAssessmentCategoryIntroScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.AssessmentCategoryIntro;

        public UITextContent Title;
        public UITextContent Content;

        public UIButton ContinueButton;

        public IEnumerator ShowCategory(CategoryDefinition category)
        {
            Title.Text.Key = category.Title.Key;
            Title.BG.color = category.BaseColor;

            Content.Text.Key = category.Description.Key;
            Content.BG.color = category.DarkColor;

            SetupButton(ContinueButton, Close);
            yield return OpenCO();
        }
    }
}
