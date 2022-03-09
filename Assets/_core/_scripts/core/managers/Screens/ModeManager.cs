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
            if (SessionMode == SessionMode.Solo) SessionMode = SessionMode.Session;
            else SessionMode = SessionMode.Solo;

            ModeText.text = SessionMode == SessionMode.Solo ? "\uf007" : "\uf500";
        }
    }
}
