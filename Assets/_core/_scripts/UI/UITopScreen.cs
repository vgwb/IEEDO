﻿using System;
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

        public UIButton InstantTranslationButton;
        public UIButton HamburgerButton;
        public UIButton SessionModeButton;

        void Start()
        {
            SetupButtonDown(InstantTranslationButton, SetTargetLocale, SetSourceLocale);
            SetupButton(HamburgerButton, () =>
            {
                SoundManager.I.PlaySfx(SfxEnum.click);
                if (Statics.Screens.Get(ScreenID.Hamburger).IsOpen)
                {
                    HamburgerButton.Text = "\uf0c9";
                    Statics.Screens.Close(ScreenID.Hamburger);
                }
                else
                {
                    HamburgerButton.Text = "\uf00d";
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
            SoundManager.I.PlaySfx(SfxEnum.click);
            var targetLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.App.ApplicationConfig.TargetLocale);
            if (targetLocale != null)
                LocalizationSettings.SelectedLocale = targetLocale;
            Statics.Analytics.App("InstantTranslation");
        }
        private void SetSourceLocale()
        {
            //SoundManager.I.PlaySfx(SfxEnum.click);
            var targetLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.App.ApplicationConfig.SourceLocale);
            if (targetLocale != null)
                LocalizationSettings.SelectedLocale = targetLocale;
            Statics.Input.UnregisterUpAction(SetSourceLocale);
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
