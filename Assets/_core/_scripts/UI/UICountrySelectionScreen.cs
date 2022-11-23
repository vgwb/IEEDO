using System.Collections;

namespace Ieedo
{
    public class UICountrySelectionScreen : UIScreen
    {
        public UIButtonsSelection ButtonsSelection;

        public IEnumerator PerformSelection()
        {
            ButtonsSelection.PerformSelection(locStrings, index => );
        }
    }
}
