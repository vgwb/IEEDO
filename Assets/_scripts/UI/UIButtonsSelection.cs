using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Ieedo
{
    public class UIButtonsSelection : UIInteractable
    {
        private UIButton[] Buttons;
        public int LatestSelectedOption;
        private bool hasSelected;

        public void HideButtons()
        {
            if (Buttons == null) Buttons = GetComponentsInChildren<UIButton>(true);
            foreach (UIButton uiButton in Buttons)
            {
                uiButton.Hide();
            }
        }

        public IEnumerator PerformSelection(Locale[] answers, System.Action<int> onSelection = null)
        {
            if (Buttons == null) Buttons = GetComponentsInChildren<UIButton>(true);
            OnSelectOption = onSelection;
            for (var i = 0; i < answers.Length; i++)
            {
                Buttons[i].Show();
                Buttons[i].AnimateAppear();
                Buttons[i].Key = new LocalizedString("UI","language_name");
                Buttons[i].GetComponentInChildren<LocalizeStringEvent>().StringReference.LocaleOverride = answers[i];
                Buttons[i].gameObject.SetActive(true);

                var selectedOption = i;
                SetupButton(Buttons[i], () => SelectOption(selectedOption));
            }

            for (int i = answers.Length; i < Buttons.Length; i++)
            {
                Buttons[i].gameObject.SetActive(false);
            }

            hasSelected = false;
            while (!hasSelected) yield return null;
        }

        public IEnumerator PerformSelection(Sprite[] answers, System.Action<int> onSelection = null)
        {
            Buttons = GetComponentsInChildren<UIButton>(true);
            OnSelectOption = onSelection;
            for (var i = 0; i < answers.Length; i++)
            {
                Buttons[i].Sprite = answers[i];
                Buttons[i].gameObject.SetActive(true);

                var selectedOption = i;
                SetupButton(Buttons[i], () => SelectOption(selectedOption));
            }

            for (int i = answers.Length; i < Buttons.Length; i++)
            {
                Buttons[i].gameObject.SetActive(false);
            }

            hasSelected = false;
            while (!hasSelected) yield return null;
        }

        public System.Action<int> OnSelectOption;
        private void SelectOption(int selectedOption)
        {
            SoundManager.I.PlaySfx(SfxEnum.click);
            OnSelectOption?.Invoke(selectedOption);
            LatestSelectedOption = selectedOption;
            hasSelected = true;
        }
    }
}
