using System.Linq;
using Lean.Gui;
using Lean.Transition.Method;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

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

        public LocalizedString Key
        {
            set
            {
                var tm = GetComponentInChildren<UIText>(true);
                tm.Key = value;
            }
        }


        public Image Shadow;
        public Image Cap;
        public LeanGraphicColor NormalColor;
        public LeanGraphicColor DownColor;

        public void SetColor(Color c)
        {
            Cap.color = c;
            NormalColor.Data.Value = c;
            DownColor.Data.Value = new Color(c.r, c.g, c.b, 1f);
        }
    }
}
