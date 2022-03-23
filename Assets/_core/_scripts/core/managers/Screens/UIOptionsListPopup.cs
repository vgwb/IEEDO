using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    public struct OptionData
    {
        public string IconText;
        public string Text;
        public Color Color;
    }

    public class UIOptionsListPopup : UIScreen
    {
        public UITooltip Tooltip;
        public UnityEngine.UI.Image EditorBG;

        public Transform ButtonsPivot;
        private UIOptionLine[] Options;
        public int LatestSelectedOption;

        public void ShowOptions(LocalizedString titleKey, List<OptionData> options, bool isTextEntry = false)
        {
            if (isTextEntry && !Application.isEditor) EditorBG.enabled = false;
            else EditorBG.enabled = true;

            if (Options == null) Options = ButtonsPivot.GetComponentsInChildren<UIOptionLine>(true);
            Tooltip.Text.Key = titleKey;
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                if (!option.IconText.IsNullOrEmpty()) {
                    Options[i].Icon.gameObject.SetActive(true);
                    Options[i].Icon.Text.SetText(Regex.Unescape(option.IconText));
                } else {
                    Options[i].Icon.gameObject.SetActive(false);
                }
                Options[i].Button.Text = option.Text;
                Options[i].Icon.BG.color = option.Color;
                Options[i].gameObject.SetActive(true);

                var selectedOption = i;
                Options[i].Button.OnClick.AddListener(() => SelectOption(selectedOption));
            }

            for (int i = options.Count; i < Options.Length; i++)
            {
                Options[i].gameObject.SetActive(false);
            }
            OpenImmediate();
        }

        public System.Action<int> OnSelectOption;
        private void SelectOption(int selectedOption)
        {
            OnSelectOption?.Invoke(selectedOption);
            LatestSelectedOption = selectedOption;
            CloseImmediate();
        }
    }
}
