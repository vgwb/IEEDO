using UnityEngine;
using UnityEngine.Localization;
using System.Collections;
using NaughtyAttributes;

namespace Ieedo.games
{
    public class ActivityManager : MonoBehaviour
    {
        public ActivityDefinition Activity;
        public System.Action<ActivityResult> OnActivityEnd;

        [BoxGroup("Local Debug")]
        public bool DebugAutoplay;
        [BoxGroup("Local Debug")]
        public int DebugStartLevel;

        protected bool Inited = false;

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

        public IEnumerator ShowQuestion(LocalizedString title, LocalizedString question, LocalizedString[] answers, Ref<int> selectedAnswerIndex)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            yield return questionScreen.ShowQuestion(title, question, answers);
            while (questionScreen.IsOpen)
                yield return null;
            selectedAnswerIndex.Value = questionScreen.LatestSelectedOption;
        }
    }
}
