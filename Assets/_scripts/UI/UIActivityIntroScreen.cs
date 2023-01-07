using System.Collections;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UIActivityIntroScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.ActivityIntro;

        public UITextContent Title;
        public UITextContent Description;

        public UIButton ContinueButton;

        public IEnumerator ShowIntro()
        {
            var gameName = $"{Statics.ActivityFlow.CurrentActivity.LocName}";
            Title.Text.Key = new LocalizedString("Activity", $"{gameName}");
            Description.Text.Key = new LocalizedString("Activity", $"{gameName}_description");
            SetupButton(ContinueButton, Continue);
            yield return OpenCO();

            while (IsOpen)
                yield return null;
        }

        private void Continue()
        {
            Close();
        }
    }
}
