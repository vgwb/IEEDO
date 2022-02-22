﻿using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{
    public struct OptionData
    {
        public string Text;
        public Color Color;
    }

    public class UIOptionsListPopup : MonoBehaviour
    {
        public UIText Title;

        public UIButton[] Buttons;
        public int LatestSelectedOption;

        public void ShowOptions(string title, List<OptionData> options)
        {
            Title.text = title;
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
            gameObject.SetActive(true);
        }

        public System.Action<int> OnSelectOption;
        private void SelectOption(int selectedOption)
        {
            OnSelectOption?.Invoke(selectedOption);
            LatestSelectedOption = selectedOption;
            gameObject.SetActive(false);
        }
    }
}