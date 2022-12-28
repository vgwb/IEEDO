using System.Collections;
using UnityEngine;

namespace Ieedo
{
    public enum SessionMode
    {
        Solo,
        Session
    }

    public class ModeManager : MonoBehaviour
    {
        public SessionMode SessionMode;

        public UIText ModeText;

        public void ToggleSessionMode()
        {
            StartCoroutine(ToggleSessionModeCO());
       }

        private IEnumerator ToggleSessionModeCO()
        {
            if (SessionMode == SessionMode.Solo)
            {
                //var answer = new Ref<int>();
                //yield return Statics.Screens.ShowQuestionFlow("UI/session_start_title", "UI/session_start_question", new[] { "UI/yes", "UI/no" }, answer);
                //if (answer.Value == 0)
                //{
                    SetSessionMode(SessionMode.Session);
                    Statics.SessionFlow.StartSessionMode();
                //}
            }
            else
            {
                if (Statics.SessionFlow.IsInsideTutorial) yield break;
                var answer = new Ref<int>();
                yield return Statics.Screens.ShowQuestionFlow("UI/session_abort_title", "UI/session_abort_question", new[] { "UI/yes", "UI/no" }, answer);
                if (answer.Value == 0)
                {
                    SetSessionMode(SessionMode.Solo);
                    Statics.SessionFlow.StopSessionMode();
                }
            }
        }

        public void SetSessionMode(SessionMode mode)
        {
            SessionMode = mode;
            ModeText.text = SessionMode == SessionMode.Solo ? "\uf007" : "\uf500";
        }
    }
}
