namespace Ieedo
{
    public class UIActivitiesScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Activities;

        public UIButton StartGameBtn;
        public UIButton StartGame2Btn;

        void Start()
        {
            var allActivities = Statics.Data.GetAll<ActivityDefinition>();

            foreach (var activityDefinition in allActivities)
            {
                // TODO: Create activity block
            }

            SetupButton(StartGameBtn, () => LaunchActivity(ActivityID.Blank));
            SetupButton(StartGame2Btn, () => LaunchActivity(ActivityID.TicTac));
        }

        private void LaunchActivity(ActivityID activity)
        {
            AppManager.I.LaunchActivity(activity);
        }
    }
}