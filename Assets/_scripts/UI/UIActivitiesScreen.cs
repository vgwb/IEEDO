using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UIActivitiesScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Activities;

        public List<UIActivityBlock> ActivityBlocks;

        public void OnEnable()
        {
            RefreshData();
        }

        void RefreshData()
        {
            var allActivities = Statics.Data.GetAll<ActivityDefinition>();
            allActivities = allActivities.Where(x => x.Enabled).OrderBy(x => x.ScoreToUnlock).ToList();

            for (var i = 0; i < allActivities.Count; i++)
            {
                ActivityBlocks[i].gameObject.SetActive(true);
                var activityDefinition = allActivities[i];

                SetupButton(ActivityBlocks[i].LaunchButton, () => LaunchActivity(activityDefinition.ID));
                ActivityBlocks[i].Title.Key = activityDefinition.Title.Key;
                ActivityBlocks[i].Image.sprite = activityDefinition.Image;

                var data = Statics.Data.Profile.Activities.FirstOrDefault(x => x.ID == activityDefinition.ID);
                if (data == null)
                {
                    // Generate data in the profile if the activity is unknown
                    data = new ActivityData
                    {
                        ID = activityDefinition.ID,
                        Unlocked = false,
                        CurrentLevel = 1,
                        Results = new ActivityResults(),
                        MaxScore = 0,
                    };
                    Statics.Data.Profile.Activities.Add(data);
                    Statics.Data.SaveProfile();
                }
                ActivityBlocks[i].ProgressBar.SetValue(data.CurrentLevel, activityDefinition.MaxLevel);
                ActivityBlocks[i].ScoreText.text =
                 "Lvl: " + data.CurrentLevel + "\n"
                 + "Score: " + data.MaxScore;

                // Check unlock state
                data.Unlocked = Statics.Data.Profile.CurrentScore >= activityDefinition.ScoreToUnlock;
                ActivityBlocks[i].LockedGO.SetActive(!data.Unlocked);

                var scoreToUnlockLoc = new LocalizedString("UI", "activity_score_to_unlock");
                scoreToUnlockLoc.Arguments = new List<object> { activityDefinition };
                ActivityBlocks[i].LockedText.Key = scoreToUnlockLoc;
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
