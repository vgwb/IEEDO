using minigame;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UIActivityIntroScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.ActivityIntro;
        public UITextContent Title;
        public UITextContent Description;
        public ui_score CurrentScore;

        public UIButton ContinueButton;

        public IEnumerator ShowIntroCO()
        {
            var gameName = $"{Statics.ActivityFlow.CurrentActivity.LocName}";
            Title.Text.Key = new LocalizedString("Activity", $"{gameName}");
            Description.Text.Key = new LocalizedString("Activity", $"{gameName}_description");

            switch (Statics.ActivityFlow.CurrentActivity.ScoreType)
            {
                case ScoreType.Highscore:
                    CurrentScore.Init(Statics.ActivityFlow.CurrentActivityData.HiScore, ui_score.ScoreLabel.hiscore);
                    break;
                case ScoreType.LevelReached:
                    CurrentScore.Init(Statics.ActivityFlow.CurrentActivityData.CurrentLevel, ui_score.ScoreLabel.level);
                    break;
                case ScoreType.NumberOfPlays:
                    CurrentScore.Init(Statics.ActivityFlow.CurrentActivityData.Results.Count(x => x.CustomData != string.Empty), ui_score.ScoreLabel.playcount);
                    break;
            }

            SetupButton(ContinueButton, Continue);
            yield return OpenCO();
        }

        private void Continue()
        {
            Close();
        }
    }
}
