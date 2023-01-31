using System.Collections;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UIActivityIntroScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.ActivityIntro;

        public UITextContent Title;
        public UITextContent Description;
        public UITextContent ScoreInfo;

        public UIButton ContinueButton;

        public IEnumerator ShowIntro()
        {
            var gameName = $"{Statics.ActivityFlow.CurrentActivity.LocName}";
            Title.Text.Key = new LocalizedString("Activity", $"{gameName}");
            Description.Text.Key = new LocalizedString("Activity", $"{gameName}_description");

            string scoreString = "";
            switch (Statics.ActivityFlow.CurrentActivity.ScoreType)
            {
                case ScoreType.Highscore:
                    scoreString = new LocalizedString("Activity", "activity_highscore").GetLocalizedString();
                    ScoreInfo.Text.SetTextRaw(scoreString + " " + Statics.ActivityFlow.CurrentActivityData.HiScore);
                    break;
                case ScoreType.LevelReached:
                    scoreString = new LocalizedString("Activity", "activity_level").GetLocalizedString();
                    ScoreInfo.Text.SetTextRaw(scoreString + " " + Statics.ActivityFlow.CurrentActivityData.CurrentLevel);
                    break;
                case ScoreType.NumberOfPlays:
                    ScoreInfo.Text.SetTextRaw(scoreString);
                    break;
            }

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
