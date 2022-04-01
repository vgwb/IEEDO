using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public LeanButton SessionModeButton;

        void Start()
        {
            SetupButtonDown(InstantTranslationButton, SetTargetLocale, SetSourceLocale);
            SetupButton(HamburgerButton, () =>
            {
                if (Statics.Screens.Get(ScreenID.Hamburger).IsOpen)
                {
                    Statics.Screens.Close(ScreenID.Hamburger);
                }
                else
                {
                    Statics.Screens.Open(ScreenID.Hamburger);
                }
            });
            SetupButton(SessionModeButton, () =>
            {
                Statics.Mode.ToggleSessionMode();
            });
            SwitchMode(TopBarMode.MainApp);
        }

        private void SetTargetLocale()
        {
            var targetLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.App.ApplicationConfig.TargetLocale);
            if (targetLocale != null)
                LocalizationSettings.SelectedLocale = targetLocale;
            Statics.Analytics.App("InstantTranslation");
        }
        private void SetSourceLocale()
        {
            var targetLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.App.ApplicationConfig.SourceLocale);
            if (targetLocale != null)
                LocalizationSettings.SelectedLocale = targetLocale;
        }

        public TopBarMode Mode;
        public void SwitchMode(TopBarMode mode)
        {
            Mode = mode;
            switch (mode)
            {
                case TopBarMode.MainApp:
                    SessionModeButton.gameObject.SetActive(true);
                    break;
                case TopBarMode.Activity:
                    SessionModeButton.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
