using System.Collections.Generic;
using System.Linq;

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

                SetupButton(ActivityBlocks[i].LaunchButton, () => LaunchActivity(activityDefinition.ID));
                ActivityBlocks[i].Title.text = activityDefinition.ID.ToString();

                var data = Statics.Data.Profile.ActivitiesData.FirstOrDefault(x => x.ID == activityDefinition.ID);
                if (data == null)
                {
                    // Generate data in the profile if the activity is unknown
                    data = new ActivityData
                    {
                        ID = activityDefinition.ID,
                        Unlocked = false,
                        Results = new ActivityResults()
                    };
                    Statics.Data.Profile.ActivitiesData.Add(data);
                    Statics.Data.SaveProfile();
                }

                // Check unlock state
                data.Unlocked = Statics.Data.Profile.CurrentScore >= activityDefinition.ScoreToUnlock;
                ActivityBlocks[i].LockedGO.SetActive(!data.Unlocked);
                ActivityBlocks[i].LockedText.text = $"{activityDefinition.ScoreToUnlock} points to unlock";
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