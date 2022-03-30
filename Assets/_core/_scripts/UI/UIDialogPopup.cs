using System.Collections;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UIDialogPopup : UIScreen
    {
        public override ScreenID ID => ScreenID.Dialog;

        public UITextContent Content;
        public UIButton Button;

        public IEnumerator ShowDialog(LocalizedString content, LocalizedString answer)
        {
            Content.Text.Key = content;
            Button.Key = answer;

            SetupButton(Button, Close);
            yield return OpenCO();
        }
    }
}
