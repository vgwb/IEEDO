using UnityEngine;
using System.Collections;

namespace Ieedo.games
{
    public class ActivityManager : MonoBehaviour
    {
        public ActivityDefinition Activity;
        public System.Action<ActivityResult> OnActivityEnd;

        public void CloseActivity(ActivityResult result)
        {
            OnActivityEnd?.Invoke(result);
        }

        public void ExternSetupActivity(int currentLevel)
        {
            SetupActivity(currentLevel);
        }

        protected virtual void SetupActivity(int currentLevel)
        {
        }

        public IEnumerator ShowQuestion(string title, string question, string[] answers, Ref<int> selectedAnswerIndex)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            yield return questionScreen.ShowQuestion(title, question, answers);
            while (questionScreen.IsOpen) yield return null;
            selectedAnswerIndex.Value = questionScreen.LatestSelectedOption;
        }
    }
}
