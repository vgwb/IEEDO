using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Ieedo
{
    public enum TopBarMode
    {
        MainApp,
        Activity,
    }

    public class UITopScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Top;

        public LeanButton InstantTranslationButton;
        public LeanButton HamburgerButton;

        public UITextContent SessionModeContent;

        void Start()
        {
            SetupButtonDown(InstantTranslationButton, StartInstantTranslate, StopInstantTranslate);
            SetupButton(HamburgerButton, () => Statics.Screens.OpenImmediate(ScreenID.Debug));
            SwitchMode(TopBarMode.MainApp);
        }

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

        public TopBarMode Mode;
        public void SwitchMode(TopBarMode mode)
        {
            Mode = mode;
            switch (mode)
            {
                case TopBarMode.MainApp:
                    SessionModeContent.gameObject.SetActive(true);
                    break;
                case TopBarMode.Activity:
                    SessionModeContent.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
