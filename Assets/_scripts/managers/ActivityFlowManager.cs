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
        public int Score;
        public ActivityResultState Result;
        public string CustomData;
        public Timestamp Timestamp = Timestamp.None;

        public ActivityResult()
        {
        }

        public ActivityResult(ActivityResultState result, int score)
        {
            Result = result;
            Score = score;
        }

    }

    public class ActivityFlowManager : MonoBehaviour
    {
        public GameObject[] ObjectsToHide;
        public ActivityDefinition CurrentActivity;
        public ActivityManager CurrentActivityManager;

        public bool IsInsideActivity => CurrentActivity != null;

        public IEnumerator LaunchActivity(ActivityID activity)
        {
            Debug.Log("LaunchActivity " + activity);
            CurrentActivity = Statics.Data.Get<ActivityDefinition>((int)activity);

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
            var activityData = Statics.Data.Profile.ActivitiesData.First(x => x.ID == CurrentActivity.ID);
            activityManager.ExternSetupActivity(activityData.CurrentLevel);
            activityManager.OnActivityEnd = CloseActivity;

            var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            uiTopScreen.SwitchMode(TopBarMode.Special_Activity);
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
            if (activityId == ActivityID.None) activityId = CurrentActivity.ID;

            if (Equals(result.Timestamp, Timestamp.None))
                result.Timestamp = Timestamp.Now;

            // Save the result of this activity and its score
            var activityData = Statics.Data.Profile.ActivitiesData.GetActivityData(activityId);

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
                existingResult.Score = result.Score;
            }

            if (isNewResult)
            {
                if (result.Result == ActivityResultState.Win)
                {
                    activityData.CurrentLevel += 1;
                }
                Statics.Score.AddScore(result.Score);
            }

            Statics.Analytics.Activity(activityData.ID.ToString(), result.Result.ToString());
        }

        public int CurrentLevel
        {
            get
            {
                var activityData = Statics.Data.Profile.ActivitiesData.First(x => x.ID == CurrentActivity.ID);
                return activityData.CurrentLevel;
            }
        }
    }
}
