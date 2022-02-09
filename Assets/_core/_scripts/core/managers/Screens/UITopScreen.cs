using Lean.Gui;

namespace Ieedo
{
    public class UITopScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Top;

        public LeanButton BackButton;
        public LeanButton AssessmentButton;
        public LeanButton LanguageButton;

        void Start()
        {
            SetupButton(AssessmentButton, () => Statics.AssessmentFlow.StartAssessment());
        }
    }
}