namespace Ieedo
{
    public class UIAssessmentCategoryIntroScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.AssessmentCategoryIntro;

        public UITextContent Title;
        public UITextContent Content;

        public UIButton ContinueButton;

        public void ShowCategory(CategoryDefinition category)
        {
            Title.Text.text = category.Title.Text;
            Title.BG.color = category.Color;

            Content.Text.text = category.Description.Text;
            Content.BG.color = category.Color.SetSaturation(0.5f);

            OpenImmediate();

            SetupButton(ContinueButton, CloseImmediate);
        }
    }
}