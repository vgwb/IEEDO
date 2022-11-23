using System.Collections;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UICountrySelectionScreen : UIScreen
    {
        public UIButtonsSelection ButtonsSelection;
        
        public IEnumerator PerformSelection()
        {
            var locStrings = new LocalizedString[5];
            
            ButtonsSelection.SetupSelection(locStrings, index => );
            
            var targetLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.App.ApplicationConfig.TargetLocale);
            if (targetLocale != null)
                LocalizationSettings.SelectedLocale = targetLocale;
        }
    }
}
