namespace Ieedo
{
    public class UIActivitiesScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Activities;

        public UIButton StartGameBtn;
        public UIButton StartGame2Btn;

        void Start()
        {
            SetupButton(StartGameBtn, () => LaunchActivity(ActivityEnum.Blank));
            SetupButton(StartGame2Btn, () => LaunchActivity(ActivityEnum.TicTac));
        }

        private void LaunchActivity(ActivityEnum activity)
        {
            AppManager.I.LaunchMinigame(activity);
        }
    }
}