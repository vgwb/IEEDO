using ArabicSupport;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace Ieedo
{
    public class UIText3D : TextMeshPro
    {
        public bool IgnoreRTL;

        public LocalizeStringEvent LocalizeStringEvent => GetComponent<LocalizeStringEvent>();
        public UnityEngine.Localization.LocalizedString Key
        {
            get => LocalizeStringEvent.StringReference;
            set => LocalizeStringEvent.StringReference = value;
        }

        public void SetTextRaw(string s)
        {
            IgnoreRTL = true;
            Text = s;
        }

        public string Text
        {
            set
            {
                if (!IgnoreRTL && LocalizationSettings.Instance.GetSelectedLocale().Identifier.Code == "ar")
                {
                    text = ArabicFixer.Fix(value);
                }
                else
                {
                    text = value;
                }
            }
        }

    }
}
