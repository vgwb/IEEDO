using Ieedo;
using UnityEngine;
using UnityEngine.Localization;
using System.Collections;
using NaughtyAttributes;
using UnityEngine.Serialization;

namespace Ieedo.games
{
    public class ActivityManager : MonoBehaviour
    {
        public ActivityDefinition Activity;
        public System.Action OnActivityEnd;

        [BoxGroup("Local Debug")]
        public bool DebugPlay;
        [BoxGroup("Local Debug")]
        public int DebugStartLevel;

        void Start()
        {
            if (DebugPlay)
            {
                Statics.ActivityFlow.CurrentActivity = Activity;
                Statics.ActivityFlow.CurrentActivityManager = this;
                SetupActivity(DebugStartLevel);
            }
        }

        public void CloseActivity()
        {
            var introScreen = Statics.Screens.Get(ScreenID.ActivityIntro);
            if (introScreen.IsOpen)
                introScreen.Close();

            var continueScreen = Statics.Screens.Get(ScreenID.ActivityContinue);
            if (continueScreen.IsOpen)
                continueScreen.Close();

            CustomCloseActivity();
            OnActivityEnd?.Invoke();
        }

        protected virtual void CustomCloseActivity()
        {
        }

        public void ExternSetupActivity(ActivityDefinition _activity, int currentLevel)
        {
            Activity = _activity;
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
