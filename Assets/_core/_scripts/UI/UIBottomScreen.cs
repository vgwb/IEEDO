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
            SetupButton(btnActivities, () =>
            {
                Statics.Screens.GoTo(ScreenID.Activities);
            });
            SetupButton(btnPillars, () =>
            {
                Statics.Screens.GoTo(ScreenID.Pillars);
            });
            SetupButton(btnCards, () =>
            {
                Statics.Screens.GoToTodoList();
            });

            ToggleSelection(btnPillars);
        }

        public void ToggleSelection(LeanButton selectedBtn)
        {
            btnActivities.interactable = btnActivities != selectedBtn;
            btnCards.interactable = btnCards != selectedBtn;
            btnPillars.interactable = btnPillars != selectedBtn;
        }
    }
}
