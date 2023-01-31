using System;
using Lean.Gui;

namespace Ieedo
{
    public class UIBottomScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Bottom;

        public LeanButton btnActivities;
        public LeanButton btnPillars;
        public LeanButton btnCards;

        public Action OnCardsClicked;

        void Start()
        {
            SetupButton(btnActivities, () =>
            {
                if (Statics.Input.IsExecutingAction) return;
                Statics.Screens.GoTo(ScreenID.Activities);
            });
            SetupButton(btnPillars, () =>
            {
                if (Statics.Input.IsExecutingAction) return;
                Statics.Screens.GoTo(ScreenID.Pillars);
            });
            SetupButton(btnCards, () =>
            {
                if (Statics.Input.IsExecutingAction) return;
                OnCardsClicked?.Invoke();
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
