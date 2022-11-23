using UnityEngine.Localization;

namespace Ieedo
{
    public class UIButtonsSelection : UIScreen
    {
        public UIButton[] Buttons;
        public int LatestSelectedOption;

        public void SetupSelection(LocalizedString[] answers, System.Action<int> onSelection)
        {
            OnSelectOption = onSelection;
            for (var i = 0; i < answers.Length; i++)
            {
                Buttons[i].Key = answers[i];
                Buttons[i].gameObject.SetActive(true);

                var selectedOption = i;
                SetupButton(Buttons[i], () => SelectOption(selectedOption));
            }

            for (int i = answers.Length; i < Buttons.Length; i++)
            {
                Buttons[i].gameObject.SetActive(false);
            }
        }

        public System.Action<int> OnSelectOption;
        private void SelectOption(int selectedOption)
        {
            SoundManager.I.PlaySfx(SfxEnum.click);
            OnSelectOption?.Invoke(selectedOption);
            LatestSelectedOption = selectedOption;
        }
    }
}
