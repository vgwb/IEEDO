using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UIActivityContinueScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.ActivityContinue;

        public UITextContent Title;
        public UITextContent Result;
        public UITextContent Score;

        public UIButton ContinueButton;
        public UIButton QuitButton;

        public IEnumerator ShowResult(ActivityResult result)
        {
            Title.Text.Key = new LocalizedString("UI", "activity_result_title");
            Result.Text.Key = new LocalizedString("UI", $"activity_result_{result.Result.ToString().ToLower()}");
            var locString = new LocalizedString("UI", "activity_result_score");
            locString.Arguments = new List<object>{result};
            Score.Text.Key = locString;

            SetupButton(ContinueButton, Continue);
            SetupButton(QuitButton, Quit);
            yield return OpenCO();
        }

        private void Quit()
        {
            Close();
            Statics.ActivityFlow.CurrentActivityManager.CloseActivity();
        }

        private void Continue()
        {
            Close();
            StartCoroutine(Statics.ActivityFlow.CurrentActivityManager.PlayNextLevel(Statics.ActivityFlow.CurrentLevel));
        }
    }
}
