using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Ieedo
{
    public class UIActivityContinueScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.ActivityContinue;

        public UITextContent Title;
        public UITextContent Content;

        public UIButton NextLevelButton;
        public UIButton QuitButton;

        public GameObject ScorePivot;
        public UITextContent CurrentScore;
        public UITextContent HighScore;

        public IEnumerator ShowResult(ActivityResult result)
        {
            var activityDef = Statics.ActivityFlow.CurrentActivity;
            var activityData = Statics.Data.Profile.Activities.GetActivityData(Statics.ActivityFlow.CurrentActivity.ID);

            switch (Statics.ActivityFlow.CurrentActivity.Type)
            {
                case ActivityType.Daily:
                    Title.Text.Key = new LocalizedString("UI", $"activity_result_daily_complete");
                    Content.Text.Key = new LocalizedString("Activity", $"{activityDef.LocName}_end");
                    break;
                case ActivityType.Game:
                    switch (activityDef.ScoreType)
                    {
                        case ScoreType.Highscore:
                            Title.Text.Key = new LocalizedString("UI", $"activity_result_game_over");
                            Content.Text.Key = new LocalizedString("Activity", $"{activityDef.LocName}_end");
                            NextLevelButton.Key = new LocalizedString("UI", "play_again");
                            break;
                        case ScoreType.LevelReached:
                            Title.Text.Key = new LocalizedString("UI", $"activity_result_game_{result.Result.ToString().ToLower()}");
                            Content.Text.Key = new LocalizedString("UI", "activity_result_play_everyday");
                            NextLevelButton.Key = new LocalizedString("UI", "play_next_level");
                            break;
                    }
                    break;
            }

            NextLevelButton.gameObject.SetActive(Statics.ActivityFlow.CurrentActivity.Type == ActivityType.Game);
            QuitButton.gameObject.SetActive(Statics.ActivityFlow.CurrentActivity.Type == ActivityType.Daily);

            ScorePivot.SetActive(Statics.ActivityFlow.CurrentActivity.ScoreType == ScoreType.Highscore);

            var scoreLoc = new LocalizedString("UI", $"activity_result_score");
            scoreLoc.Arguments = new List<object> { result.Score };
            scoreLoc.Add("Score", new IntVariable { Value = result.Score });
            CurrentScore.Text.Key = scoreLoc;

            var highscoreLoc = new LocalizedString("UI", $"activity_result_highscore");
            highscoreLoc.Arguments = new List<object> { activityData.MaxScore };
            highscoreLoc.Add("HighScore", new IntVariable { Value = activityData.MaxScore });
            HighScore.Text.Key = highscoreLoc;

            SetupButton(NextLevelButton, () => Continue());
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
