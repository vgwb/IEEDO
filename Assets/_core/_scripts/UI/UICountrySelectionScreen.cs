using System.Collections;
using System.Linq;

namespace Ieedo
{
    public class UICountrySelectionScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.CountrySelection;

        public UIButtonsSelection ButtonsSelection;

        public IEnumerator PerformSelection(ProfileData profileData)
        {
            var availableCountries = Statics.Data.GetAll<CountryDefinition>();
            var flags = availableCountries.Select(x => x.Flag);
            yield return ButtonsSelection.PerformSelection(flags.ToArray());
            var targetCountry = availableCountries[ButtonsSelection.LatestSelectedOption];
            if (targetCountry != null)
            {
                profileData.Description.Country = targetCountry.Code;
            }
        }
    }
}
