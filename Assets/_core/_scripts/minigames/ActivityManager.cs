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

        public IEnumerator CompleteLevel(ActivityResult result)
        {
            var continueScreen = Statics.Screens.Get(ScreenID.ActivityContinue) as UIActivityContinueScreen;
            yield return continueScreen.ShowResult(result);
        }

        public IEnumerator ShowQuestion(LocalizedString title, LocalizedString question, LocalizedString[] answers, Ref<int> selectedAnswerIndex)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            yield return questionScreen.ShowQuestionFlow(title, question, answers, selectedAnswerIndex);
        }
    }
}
