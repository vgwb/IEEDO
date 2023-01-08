using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

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
            allActivities = allActivities.Where(x => x.Enabled).OrderBy(x => x.PointsToUnlock).ToList();

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
                        HiScore = 0,
                    };
                    Statics.Data.Profile.Activities.Add(data);
                    Statics.Data.SaveProfile();
                }
                //                Debug.Log(activityDefinition.ID + " / " + activityDefinition.MaxLevel);
                //ActivityBlocks[i].ProgressBar.SetValue(data.CurrentLevel, activityDefinition.MaxLevel);


                var scoreTypeLoc = new LocalizedString("Activity", $"activity_scoretype_{activityDefinition.ScoreType.ToString().ToLower()}");
                switch (activityDefinition.ScoreType)
                {
                    case ScoreType.Highscore:
                        scoreTypeLoc.Arguments = new List<object> { data.HiScore };
                        scoreTypeLoc.Add("HighScore", new IntVariable { Value = data.HiScore });
                        break;
                    case ScoreType.LevelReached:
                        scoreTypeLoc.Arguments = new List<object> { data.CurrentLevel, activityDefinition.MaxLevel };
                        scoreTypeLoc.Add("CurrentLevel", new IntVariable { Value = data.CurrentLevel });
                        scoreTypeLoc.Add("MaxLevel", new IntVariable { Value = activityDefinition.MaxLevel });
                        break;
                    case ScoreType.NumberOfPlays:
                        scoreTypeLoc.Arguments = new List<object> { data.Results.Count };
                        scoreTypeLoc.Add("Count", new IntVariable { Value = data.Results.Count });
                        break;
                    case ScoreType.None:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                ActivityBlocks[i].ScoreText.Key = scoreTypeLoc;

                // Check unlock state
                if (activityDefinition.Available)
                {
                    data.Unlocked = Statics.Data.Profile.CurrentPoints >= activityDefinition.PointsToUnlock;
                    ActivityBlocks[i].LockedGO.SetActive(!data.Unlocked);

                    var pointsToUnlockLoc = new LocalizedString("Activity", "activity_points_to_unlock");
                    pointsToUnlockLoc.Arguments = new List<object> { activityDefinition };
                    ActivityBlocks[i].LockedText.Key = pointsToUnlockLoc;
                }
                else
                {
                    data.Unlocked = false;
                    ActivityBlocks[i].LockedText.Key = new LocalizedString("Activity", "activity_available_soon");
                }
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
