using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    public struct OptionData
    {
        public string Text;
        public Color Color;
    }

    public class UIOptionsListPopup : UIScreen
    {
        public UIText Title;

        public Transform ButtonsPivot;
        private UIButton[] Buttons;
        public int LatestSelectedOption;

        public void ShowOptions(LocalizedString titleKey, List<OptionData> options)
        {
            if (Buttons == null) Buttons = ButtonsPivot.GetComponentsInChildren<UIButton>(true);
            Title.text = titleKey.GetLocalizedString();
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                Buttons[i].Text = option.Text;
                Buttons[i].SetColor(option.Color);
                Buttons[i].gameObject.SetActive(true);

                var selectedOption = i;
                Buttons[i].OnClick.AddListener(() => SelectOption(selectedOption));
            }

            for (int i = options.Count; i < Buttons.Length; i++)
            {
                Buttons[i].gameObject.SetActive(false);
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
