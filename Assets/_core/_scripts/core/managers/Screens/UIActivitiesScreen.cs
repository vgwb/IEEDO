using System.Collections.Generic;

namespace Ieedo
{
    public class UIActivitiesScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Activities;

        public List<UIActivityBlock> ActivityBlocks;

        void Start()
        {
            var allActivities = Statics.Data.GetAll<ActivityDefinition>();

            for (var i = 0; i < allActivities.Count; i++)
            {
                ActivityBlocks[i].gameObject.SetActive(true);
                var activityDefinition = allActivities[i];
                ActivityBlocks[i].Title.text = activityDefinition.ID.ToString();
                SetupButton(ActivityBlocks[i].LaunchButton, () => LaunchActivity(activityDefinition.ID));
                ActivityBlocks[i].Title.text = activityDefinition.ID.ToString();
            }

            for (int i = allActivities.Count; i < ActivityBlocks.Count; i++)
            {
                ActivityBlocks[i].gameObject.SetActive(false);
            }
        }

        private void LaunchActivity(ActivityID activity)
        {
            StartCoroutine(Statics.ActivityFlow.LaunchActivity(activity));
        }
    }
}