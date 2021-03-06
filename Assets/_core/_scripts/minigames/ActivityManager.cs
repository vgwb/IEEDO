using UnityEngine;
using UnityEngine.Localization;
using System.Collections;
using NaughtyAttributes;

namespace Ieedo.games
{
    public class ActivityManager : MonoBehaviour
    {
        public ActivityDefinition Activity;
        public System.Action OnActivityEnd;

        [BoxGroup("Local Debug")]
        public bool DebugAutoplay;
        [BoxGroup("Local Debug")]
        public int DebugStartLevel;

        protected bool Inited = false;

        // TODO: remove this result, not needed anymore
        public void CloseActivity(ActivityResult result = null)
        {
            OnActivityEnd?.Invoke();
        }

        public void ExternSetupActivity(int currentLevel)
        {
            SetupActivity(currentLevel);
        }

        protected virtual void SetupActivity(int currentLevel)
        {
        }

        public IEnumerator CompleteActivity(ActivityResult result)
        {
            Statics.ActivityFlow.RegisterResult(result);
            var continueScreen = Statics.Screens.Get(ScreenID.ActivityContinue) as UIActivityContinueScreen;
            yield return continueScreen.ShowResult(result);
        }

        public virtual IEnumerator PlayNextLevel(int _currentLevel)
        {
            yield break;
        }

        public IEnumerator ShowQuestion(LocalizedString title, LocalizedString question, LocalizedString[] answers, Ref<int> selectedAnswerIndex)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            yield return questionScreen.ShowQuestionFlow(title, question, answers, selectedAnswerIndex);
        }
    }
}
