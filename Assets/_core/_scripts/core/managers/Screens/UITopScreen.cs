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

        public LeanButton BackButton;
        public LeanButton SwitchModeButton;
        public LeanButton LanguageButton;
        public LeanButton DebugButton;

        void Start()
        {
            SetupButton(SwitchModeButton, SwitchMode);
            SetupButton(LanguageButton, StartSelectionLanguage);
            SetupButton(DebugButton, () => Statics.Screens.OpenImmediate(ScreenID.Debug));
        }

        public UIOptionsListPopup OptionsListPopup;

        private void SwitchMode()
        {
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            uiPillarsScreen.SwitchViewMode(uiPillarsScreen.ViewMode == PillarsViewMode.Categories ? PillarsViewMode.Review : PillarsViewMode.Categories);
        }

        private void StartSelectionLanguage()
        {
            StartCoroutine(SelectionLanguageCO());
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