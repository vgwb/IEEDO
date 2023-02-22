using System;
using System.Collections;
using System.Linq;
using Ieedo.games;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ieedo
{
    public enum ActivityResultState
    {
        NONE = 0,
        Win,
        Lose,
        Quit,
    }

    [System.Serializable]
    public class ActivityResult
    {
        public int PlayedLevel;
        public int Score;
        public int Points;
        public ActivityResultState Result;
        public string CustomData;
        public Timestamp Timestamp = Timestamp.None;

        public ActivityResult()
        {
        }

        public ActivityResult(ActivityResultState result, int points, int score = 0, int level = 0)
        {
            Result = result;
            Points = points;
            Score = score;
            PlayedLevel = level;
        }
    }

    public class ActivityFlowManager : MonoBehaviour
    {
        public GameObject[] ObjectsToHide;
        public ActivityDefinition CurrentActivity;
        public ActivityData CurrentActivityData;
        public ActivityManager CurrentActivityManager;

        public bool IsInsideActivity => CurrentActivity != null;

        public IEnumerator LaunchActivityCO(ActivityID activity)
        {
            Debug.Log("LaunchActivity " + activity);
            CurrentActivity = Statics.Data.Get<ActivityDefinition>((int)activity);

            CurrentActivityData = Statics.Data.Profile.Activities.First(x => x.ID == CurrentActivity.ID);

            var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            uiTopScreen.SwitchMode(TopBarMode.Special_Activity);

            var introScreen = Statics.Screens.Get(ScreenID.ActivityIntro) as UIActivityIntroScreen;
            introScreen.gameObject.SetActive(true);
            yield return introScreen.ShowIntroCO();

            var async = SceneManager.LoadSceneAsync(CurrentActivity.SceneName, LoadSceneMode.Additive);
            while (!async.isDone)
                yield return null;
            foreach (var o in ObjectsToHide)
                o.SetActive(false);

            var activityManager = FindObjectOfType<ActivityManager>();
            if (activityManager == null)
            {
                Debug.LogError("No ActivityLogic script could be found. Did you add it to the game scene?");
                yield break;
            }
            CurrentActivityManager = activityManager;

            activityManager.ExternSetupActivity(CurrentActivity, CurrentActivityData.CurrentLevel);
            activityManager.OnActivityEnd = CloseActivity;


            while (introScreen.IsOpen)
                yield return null;


        }

        private void CloseActivity()
        {
            StartCoroutine(CloseActivityCO());
        }

        private IEnumerator CloseActivityCO()
        {
            if (CurrentActivity != null)
            {
                var async = SceneManager.UnloadSceneAsync(CurrentActivity.SceneName);
                while (!async.isDone)
                    yield return null;
            }

            foreach (var o in ObjectsToHide)
                o.SetActive(true);

            //var resultScreen = Statics.Screens.Get(ScreenID.ActivityResult) as UIActivityResultScreen;
            //yield return resultScreen.ShowResult(result);

            CurrentActivity = null;
            CurrentActivityManager = null;

            var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            uiTopScreen.SwitchMode(TopBarMode.MainSection);
        }

        public void RegisterResult(ActivityResult result, ActivityID activityId = ActivityID.None)
        {
            Debug.Log("RegisterResult " + result.Score);
            if (activityId == ActivityID.None)
                activityId = CurrentActivity.ID;

            if (Equals(result.Timestamp, Timestamp.None))
                result.Timestamp = Timestamp.Now;

            // Save the result of this activity and its score
            var activityData = Statics.Data.Profile.Activities.GetActivityData(activityId);

            bool isNewResult = false;
            var existingResult = activityData.Results.FirstOrDefault(x => x.Timestamp.Equals(result.Timestamp));
            if (existingResult == null)
            {
                activityData.Results.Add(result);
                isNewResult = true;
            }
            else
            {
                existingResult.Result = result.Result;
                existingResult.CustomData = result.CustomData;
                existingResult.Points = result.Points;
                existingResult.Score = result.Score;
                existingResult.PlayedLevel = Statics.ActivityFlow.CurrentLevel;
            }

            if (isNewResult)
            {
                if (result.Result == ActivityResultState.Win)
                {
                    activityData.CurrentLevel += 1;
                    Statics.Points.AddPoints(result.Points);
                }
                else
                {
                    Statics.Points.AddPoints(result.Points);
                }

                activityData.HiScore = activityData.Results.Max(x => x.Score);
            }
            Statics.Data.SaveProfile();
            Statics.Analytics.Activity(activityData.ID.ToString(), result.Result.ToString());
        }

        public int CurrentLevel
        {
            get
            {
                var activityData = Statics.Data.Profile.Activities.First(x => x.ID == CurrentActivity.ID);
                return activityData.CurrentLevel;
            }
        }
    }
}
