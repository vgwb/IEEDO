using System.Collections;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Ieedo
{
    public class UIDialogPopup : UIScreen
    {
        public override ScreenID ID => ScreenID.Dialog;

        public UITextContent Content;
        public UIButton Button;
        public Image BGImage;

        public IEnumerator ShowDialog(LocalizedString content, LocalizedString answer)
        {
            Content.Text.Key = content;
            Button.Key = answer;

            BGImage.color = Statics.Art.UIColor.BaseColor;
            Content.BG.color =Statics.Art.UIColor.DarkColor;

            SetupButton(Button, Close);
            yield return OpenCO();
        }
    }
}
