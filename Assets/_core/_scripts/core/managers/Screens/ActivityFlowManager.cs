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

        public string CustomData;
        public int ValueContent;
        public Timestamp Timestamp;

        public ActivityResult()
        {
        }

        public ActivityResult(int valueContent)
        {
            ValueContent = valueContent;
        }

    }

    public class ActivityFlowManager : MonoBehaviour
    {
        public ActivityDefinition CurrentActivity;

        public IEnumerator LaunchActivity(ActivityID activity)
        {
            CurrentActivity = Statics.Data.Get<ActivityDefinition>((int)activity);

            var async = SceneManager.LoadSceneAsync(CurrentActivity.SceneName, LoadSceneMode.Additive);
            while (!async.isDone) yield return null;

            var activityManager = FindObjectOfType<ActivityLogic>();
            if (activityManager == null)
            {
                Debug.LogError("No ActivityLogic script could be found. Did you add it to the game scene?");
                yield break;
            }

            activityManager.OnActivityEnd = CloseActivity;
        }

        private void CloseActivity(ActivityResult result)
        {
            if (CurrentActivity != null)
            {
                SceneManager.UnloadSceneAsync(CurrentActivity.SceneName);
            }

            // Save the result of this activity
            result.Timestamp = Timestamp.Now;
            var activityData = Statics.Data.Profile.ActivitiesData.First(x => x.ID == CurrentActivity.ID);
            activityData.Unlocked = false;
            activityData.Results.Add(result);
            Statics.Data.SaveProfile();

            CurrentActivity = null;
        }
    }
}