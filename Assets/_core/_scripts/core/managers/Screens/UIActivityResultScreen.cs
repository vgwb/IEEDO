using System.Collections;

namespace Ieedo
{
    public class UIActivityResultScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.ActivityResult;

        public UITextContent Title;
        public UITextContent Result;
        public UITextContent Score;

        public UIButton ContinueButton;

        public IEnumerator ShowResult(ActivityResult result)
        {
            Result.Text.text = result.Result.ToString();
            Score.Text.text = $"Score: +{result.Score}";

            SetupButton(ContinueButton, Close);
            yield return OpenCO();
        }
    }
}