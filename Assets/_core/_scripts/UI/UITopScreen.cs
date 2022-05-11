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
        MainSection,
        SpecialSection_WithSession,
        SpecialSection_NoSession,
    }

    public class UITopScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Top;

        public UIButton InstantTranslationButton;
        public UIButton HamburgerButton;
        public UIButton SessionModeButton;

        public void SetHamburgerIcon()
        {
            HamburgerButton.Text = "\uf0c9";
        }

        public void SetCrossIcon()
        {
            HamburgerButton.Text = "\uf00d";
        }

        void Start()
        {
            SetupButtonDown(InstantTranslationButton, SetTargetLocale, SetSourceLocale);
            SetupButton(HamburgerButton, () =>
            {
                SoundManager.I.PlaySfx(SfxEnum.click);
                switch (Mode)
                {
                    case TopBarMode.MainSection:
                        if (Statics.Screens.Get(ScreenID.Hamburger).IsOpen)
                        {
                            SetHamburgerIcon();
                            Statics.Screens.Close(ScreenID.Hamburger);
                        }
                        else
                        {
                            SetCrossIcon();
                            Statics.Screens.Open(ScreenID.Hamburger);
                        }
                        break;
                    case TopBarMode.SpecialSection_WithSession:
                    case TopBarMode.SpecialSection_NoSession:
                        var hamburgerScreen = Statics.Screens.Get(ScreenID.Hamburger) as UIHamburgerScreen;
                        StartCoroutine(hamburgerScreen.AbortSpecialSectionCO());
                        break;
                }

            });
            SetupButton(SessionModeButton, () =>
            {
                Statics.Mode.ToggleSessionMode();
            });
            SwitchMode(TopBarMode.MainSection);
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
                case TopBarMode.MainSection:
                    SetHamburgerIcon();
                    SessionModeButton.gameObject.SetActive(true);
                    break;
                case TopBarMode.SpecialSection_WithSession:
                    SetCrossIcon();
                    SessionModeButton.gameObject.SetActive(true);
                    break;
                case TopBarMode.SpecialSection_NoSession:
                    SetCrossIcon();
                    SessionModeButton.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
