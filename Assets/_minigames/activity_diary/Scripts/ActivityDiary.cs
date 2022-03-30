using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ieedo.games.diary
{
    public class Page
    {
        public string Text;
        public DateTime Date;
    }

    public class ActivityDiary : ActivityManager
    {
        public List<Page> Pages = new List<Page>();

        protected override void SetupActivity(int currentLevel)
        {
            Debug.Log($"Starting ActivityDiary");

            var activityData = Statics.Data.Profile.ActivitiesData.GetActivityData(ActivityID.Diary);

            Pages.Clear();
            foreach (ActivityResult activityResult in  activityData.Results)
            {
                Pages.Add(new Page
                {
                    Text = activityResult.CustomData,
                    Date = activityResult.Timestamp.Date,
                });
            }

            foreach (Page page in Pages)
            {
                Debug.LogError(page.Date + ": " + page.Text);
            }
        }

        public void OnComplete(string todayText)
        {
            Debug.LogError("Updating page of " + Timestamp.Today.Date + " with text " + todayText);
            var activityData = Statics.Data.Profile.ActivitiesData.GetActivityData(ActivityID.Diary);
            var todayResult = activityData.Results.FirstOrDefault(x => x.Timestamp.Equals(Timestamp.Today));
            if (todayResult == null)
            {
                todayResult = new ActivityResult(ActivityResultState.Win, 10);
                todayResult.Timestamp = Timestamp.Today;
            }
            todayResult.CustomData = todayText;
            StartCoroutine(CompleteActivity(todayResult));
        }

    }
}
