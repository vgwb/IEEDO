using Lean.Gui;

namespace Ieedo
{
    public class UIBottomScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Bottom;

        public LeanButton btnActivities;
        public LeanButton btnPillars;
        public LeanButton btnCards;

        void Start()
        {
            SetupButton(btnActivities, () => GoTo(ScreenID.Activities));
            SetupButton(btnPillars, () => GoTo(ScreenID.Pillars));
            SetupButton(btnCards, () => GoTo(ScreenID.CurrentCards));
        }
    }
}