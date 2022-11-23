using System.Collections;
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
                LocalizationSettings.SelectedLocale = chosenLocale;
                profileData.Description.Language = chosenLocale.Identifier.Code;
            }
        }
    }
}
