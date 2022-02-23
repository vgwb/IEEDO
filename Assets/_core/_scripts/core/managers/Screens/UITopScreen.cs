using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Ieedo
{
    public class UITopScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Top;

        public LeanButton InstantTranslationButton;
        public LeanButton DebugButton;

        void Start()
        {
            SetupButtonDown(InstantTranslationButton, StartInstantTranslate, StopInstantTranslate);
            SetupButton(DebugButton, () => Statics.Screens.OpenImmediate(ScreenID.Debug));
        }

        public UIOptionsListPopup OptionsListPopup;

        private void StartInstantTranslate()
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            LocalizationSettings.SelectedLocale = locales[1];
        }
        private void StopInstantTranslate()
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            LocalizationSettings.SelectedLocale = locales[0];
        }

        private IEnumerator SelectionLanguageCO()
        {
            var options = new List<OptionData>();
            var locales = LocalizationSettings.AvailableLocales.Locales;
            foreach (var locale in locales)
            {
                options.Add(
                    new OptionData
                    {
                        Text = locale.LocaleName,
                        Color = Color.white,
                    }
                );
            }
            OptionsListPopup.ShowOptions("Choose Language", options);
            while (OptionsListPopup.isActiveAndEnabled) yield return null;
            var selectedLocale = locales[OptionsListPopup.LatestSelectedOption];
            LocalizationSettings.SelectedLocale = selectedLocale;
        }
    }
}