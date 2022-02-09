namespace Ieedo
{
    public class UIActivitiesScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Activities;

        public UIButton StartGameBtn;

        void Start()
        {
            // TODO: Stefano place your logic here
            SetupButton(StartGameBtn, () => DoStuff());
        }

        private void DoStuff()
        {
            AppManager.I.LaunchMinigame();
        }
    }
}