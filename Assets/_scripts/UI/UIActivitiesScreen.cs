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

            int ActivitiySorting(ActivityDefinition x)
            {
                if (!x.Available)
                    return 100000;
                return x.PointsToUnlock;
            }

            allActivities = allActivities.Where(x => x.Enabled).OrderBy(ActivitiySorting).ToList();

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

                LocalizedString ScoreLabel;
                string ScoreText = "";
                switch (activityDefinition.ScoreType)
                {
                    case ScoreType.Highscore:
                        ScoreLabel = new LocalizedString("Activity", "activity_highscore");
                        ScoreText = data.HiScore.ToString();
                        break;
                    case ScoreType.LevelReached:
                        ScoreLabel = new LocalizedString("Activity", "activity_level");
                        ScoreText = data.CurrentLevel + " / " + activityDefinition.MaxLevel;
                        break;
                    case ScoreType.NumberOfPlays:
                        ScoreLabel = new LocalizedString("Activity", "activity_played");
                        ScoreText = data.Results.Count.ToString();
                        break;
                    case ScoreType.None:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                ActivityBlocks[i].ScoreText.Key = ScoreLabel;
                ActivityBlocks[i].ScoreValue.SetTextRaw(ScoreText);


                // Check unlock state
                if (activityDefinition.Available)
                {
                    //Debug.Log("ciccio " + Statics.App.ApplicationConfig.GetPointsSymbolString());
                    data.Unlocked = Statics.Data.Profile.CurrentPoints >= activityDefinition.PointsToUnlock;
                    ActivityBlocks[i].LockedGO.SetActive(!data.Unlocked);

                    var pointsToUnlockLoc = new LocalizedString("Activity", "activity_to_unlock");
                    //                    pointsToUnlockLoc.Arguments = new List<object> { activityDefinition.PointsToUnlock };
                    string points = activityDefinition.PointsToUnlock + " " + Statics.App.ApplicationConfig.GetPointsSymbolString();
                    pointsToUnlockLoc.Add("points", new StringVariable { Value = points });
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
