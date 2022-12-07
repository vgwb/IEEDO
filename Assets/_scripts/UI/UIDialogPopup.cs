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

            BGImage.color = Statics.Art.ToBG(Statics.Art.UIColor.Color);
            Content.BG.color = Statics.Art.ToTitle(Statics.Art.UIColor.Color);
            Button.SetColor(Statics.Art.ToTitle(Statics.Art.UIColor.Color));

            SetupButton(Button, Close);
            yield return OpenCO();
        }
    }
}
