using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Ieedo
{
    public class UILanguageSelectionScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.LanguageSelection;

        public UIButtonsSelection ButtonsSelection;

        public IEnumerator PerformSelection(ProfileData profileData)
        {
            var availableLocales = LocalizationSettings.AvailableLocales.Locales;
            yield return ButtonsSelection.PerformSelection(availableLocales.ToArray());
            var chosenLocale = availableLocales[ButtonsSelection.LatestSelectedOption];
            if (chosenLocale != null)
            {
                profileData.Description.NativeLocale = chosenLocale.Identifier.Code;

                // Temporarely set this as the locale, for next screen
                var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.Data.Profile.Description.NativeLocale);
                LocalizationSettings.SelectedLocale = locale;
            }
        }
    }
}
