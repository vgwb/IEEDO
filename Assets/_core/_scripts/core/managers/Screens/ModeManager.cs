using UnityEngine;

namespace Ieedo
{
    public enum Mode
    {
        Solo,
        Session
    }

    public class ModeManager : MonoBehaviour
    {
        public Mode CurrentMode;

        public UIText ModeText;

        public void ToggleMode()
        {
            if (CurrentMode == Mode.Solo) CurrentMode = Mode.Session;
            else CurrentMode = Mode.Solo;

            ModeText.text = CurrentMode == Mode.Solo ? "\uf599" : "\uf599 \uf599";
        }
    }
}