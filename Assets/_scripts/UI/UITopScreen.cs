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
        MainSection,
        Special_Assessment,
        Special_Activity,
        Special_CardCreation,
    }

    public class UITopScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Top;

        public UIButton InstantTranslationButton;
        public UIButton HamburgerButton;
        public UIButton SessionModeButton;

        public Action OnTargetLocaleSwitched;
        public Action OnSessionModeToggled;

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
            SetupButtonDown(InstantTranslationButton, UseNativeLocale, UseHostLocale);
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
                    case TopBarMode.Special_Assessment:
                    case TopBarMode.Special_Activity:
                    case TopBarMode.Special_CardCreation:
                        var hamburgerScreen = Statics.Screens.Get(ScreenID.Hamburger) as UIHamburgerScreen;
                        StartCoroutine(hamburgerScreen.AbortSpecialSectionCO());
                        break;
                }

            });
            SetupButton(SessionModeButton, () =>
            {
                OnSessionModeToggled?.Invoke();
                Statics.Mode.ToggleSessionMode();
            });
            SwitchMode(TopBarMode.MainSection);
        }

        private void UseNativeLocale()
        {
            SoundManager.I.PlaySfx(SfxEnum.click);
            var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.Data.Profile.Settings.NativeLocale);
            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
            Statics.Analytics.App("InstantTranslation");

            OnTargetLocaleSwitched?.Invoke();
        }
        private void UseHostLocale()
        {
            //SoundManager.I.PlaySfx(SfxEnum.click);
            var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.Data.Profile.Settings.HostCountryLocale);
            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
            Statics.Input.UnregisterUpAction(UseHostLocale);
        }

        public TopBarMode Mode;
        public void SwitchMode(TopBarMode mode)
        {
            Mode = mode;
            switch (mode)
            {
                case TopBarMode.MainSection:
                    SetHamburgerIcon();
                    SessionModeButton.interactable = true;
                    break;
                case TopBarMode.Special_Assessment:
                    SetCrossIcon();
                    SessionModeButton.interactable = false;
                    break;
                case TopBarMode.Special_Activity:
                case TopBarMode.Special_CardCreation:
                    SetCrossIcon();
                    SessionModeButton.interactable = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
