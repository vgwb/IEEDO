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
                ToggleSelection(btnActivities);
                GoTo(ScreenID.Activities);
            });
            SetupButton(btnPillars, () =>
            {
                ToggleSelection(btnPillars);
                GoTo(ScreenID.Pillars);
            });
            SetupButton(btnCards, () =>
            {
                ToggleSelection(btnCards);
                var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
                uiCardListScreen.LoadToDoCards();
                uiCardListScreen.KeepPillars = false;
                GoTo(ScreenID.CardList);
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
