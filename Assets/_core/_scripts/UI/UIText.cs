using ArabicSupport;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace Ieedo
{
    public class UIText : TextMeshProUGUI
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
                    alignment = RighterizeAlignment(alignment);
                    text = ArabicFixer.Fix(value);
                }
                else
                {
                    if (!IgnoreRTL) alignment = LeftizeAlignment(alignment);
                    text = value;
                }
            }
        }

        public static TextAlignmentOptions LeftizeAlignment(TextAlignmentOptions alignment)
        {
            switch (alignment)
            {
                case TextAlignmentOptions.Right:
                    return TextAlignmentOptions.Left;
                case TextAlignmentOptions.BaselineRight:
                    return TextAlignmentOptions.BaselineLeft;
                case TextAlignmentOptions.BottomRight:
                    return TextAlignmentOptions.BottomLeft;
                case TextAlignmentOptions.CaplineRight:
                    return TextAlignmentOptions.CaplineLeft;
                case TextAlignmentOptions.MidlineRight:
                    return TextAlignmentOptions.MidlineLeft;
                case TextAlignmentOptions.TopRight:
                    return TextAlignmentOptions.TopLeft;
            }
            return alignment;
        }

        public static TextAlignmentOptions RighterizeAlignment(TextAlignmentOptions alignment)
        {
            switch (alignment)
            {
                case TextAlignmentOptions.Left:
                    return TextAlignmentOptions.Right;
                case TextAlignmentOptions.BaselineLeft:
                    return TextAlignmentOptions.BaselineRight;
                case TextAlignmentOptions.BottomLeft:
                    return TextAlignmentOptions.BottomRight;
                case TextAlignmentOptions.CaplineLeft:
                    return TextAlignmentOptions.CaplineRight;
                case TextAlignmentOptions.MidlineLeft:
                    return TextAlignmentOptions.MidlineRight;
                case TextAlignmentOptions.TopLeft:
                    return TextAlignmentOptions.TopRight;
            }
            return alignment;
        }
    }
}
