using System.Collections;
using Ieedo.games;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ieedo
{
    [System.Serializable]
    public class ActivityResult
    {
        public string TextContent;
        public int ValueContent;
        public Timestamp Timestamp;
        public ActivityID ActivityID;

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

            var activityManager = FindObjectOfType<Minigame>();
            if (activityManager == null)
            {
                Debug.LogError("No Minigame script could be found. Did you add it to the game scene?");
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

            // Save the result of this minigame
            result.ActivityID = CurrentActivity.ID;
            result.Timestamp = Timestamp.Now;
            Statics.Data.Profile.ActivityResults.Add(result);
            Statics.Data.SaveProfile();

            CurrentActivity = null;
        }
    }
}