using Lean.Gui;

namespace Ieedo
{
    public class UIButton : LeanButton
    {
        public string Text
        {
            set
            {
                var tm = GetComponentInChildren<UIText>(true);
                tm.text = value;
            }
        }
    }
}