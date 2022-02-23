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
        public Timestamp Timestamp;

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

        public IEnumerator LaunchActivity(ActivityID activity)
        {
            CurrentActivity = Statics.Data.Get<ActivityDefinition>((int)activity);

            var async = SceneManager.LoadSceneAsync(CurrentActivity.SceneName, LoadSceneMode.Additive);
            while (!async.isDone) yield return null;
            foreach (var o in ObjectsToHide) o.SetActive(false);

            var activityManager = FindObjectOfType<ActivityLogic>();
            if (activityManager == null)
            {
                Debug.LogError("No ActivityLogic script could be found. Did you add it to the game scene?");
                yield break;
            }

            var uiPillarsScreen =  Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            uiPillarsScreen.Scene3D.SetActive(false);

            var activityData = Statics.Data.Profile.ActivitiesData.First(x => x.ID == CurrentActivity.ID);
            activityManager.ExternSetupActivity(activityData.CurrentLevel);
            activityManager.OnActivityEnd = CloseActivity;
        }

        private void CloseActivity(ActivityResult result)
        {
            StartCoroutine(CloseActivityCO(result));
        }

        private IEnumerator CloseActivityCO(ActivityResult result)
        {
            if (CurrentActivity != null)
            {
                var async = SceneManager.UnloadSceneAsync(CurrentActivity.SceneName);
                while (!async.isDone) yield return null;
            }

            foreach (var o in ObjectsToHide) o.SetActive(true);

            var resultScreen = Statics.Screens.Get(ScreenID.ActivityResult) as UIActivityResultScreen;
            yield return resultScreen.ShowResult(result);

            // Save the result of this activity and its score
            result.Timestamp = Timestamp.Now;
            var activityData = Statics.Data.Profile.ActivitiesData.First(x => x.ID == CurrentActivity.ID);
            activityData.Results.Add(result);

            if (result.Result == ActivityResultState.Win)
            {
                activityData.CurrentLevel += 1;
            }

            Statics.Score.AddScore(result.Score);
            Statics.Data.SaveProfile();

            CurrentActivity = null;

            var uiPillarsScreen =  Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            uiPillarsScreen.Scene3D.SetActive(true);
        }
    }
}