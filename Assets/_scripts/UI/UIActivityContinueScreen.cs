using minigame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UIActivityContinueScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.ActivityContinue;

        public UITextContent Title;
        public UITextContent Content;

        public UIButton NextLevelButton;
        public UIButton QuitButton;

        public GameObject ScoresContainer;
        public ui_score CurrentScore;
        public ui_score HighScore;

        public IEnumerator ShowResult(ActivityResult result)
        {
            var activityDef = Statics.ActivityFlow.CurrentActivity;
            var activityData = Statics.Data.Profile.Activities.GetActivityData(Statics.ActivityFlow.CurrentActivity.ID);

            switch (Statics.ActivityFlow.CurrentActivity.Type)
            {
                case ActivityType.Daily:
                    Title.Text.Key = new LocalizedString("Activity", $"activity_result_daily_complete");
                    Content.Text.Key = new LocalizedString("Activity", $"daily_write_diary_end");
                    NextLevelButton.gameObject.SetActive(false);
                    ScoresContainer.SetActive(false);
                    break;
                case ActivityType.Game:
                    switch (activityDef.ScoreType)
                    {
                        case ScoreType.Highscore:
                            Title.Text.Key = new LocalizedString("Activity", "activity_result_game_over");
                            Content.Text.Key = new LocalizedString("Activity", "activity_play_everyday");
                            NextLevelButton.Key = new LocalizedString("Activity", "activity_play_again");

                            CurrentScore.Init(result.Score, ui_score.ScoreLabel.score);
                            HighScore.Init(activityData.HiScore, ui_score.ScoreLabel.hiscore);
                            HighScore.Show(true);
                            ScoresContainer.SetActive(true);
                            break;
                        case ScoreType.NumberOfPlays:
                            Title.Text.Key = new LocalizedString("Activity", $"activity_result_game_{result.Result.ToString().ToLower()}");
                            Content.Text.Key = new LocalizedString("Activity", "activity_play_everyday");
                            NextLevelButton.Key = new LocalizedString("Activity", "activity_play_again");
                            ScoresContainer.SetActive(false);
                            break;
                        case ScoreType.LevelReached:
                            Title.Text.Key = new LocalizedString("Activity", $"activity_completed");
                            Content.Text.Key = new LocalizedString("Activity", "activity_play_everyday");
                            NextLevelButton.Key = new LocalizedString("Activity", "activity_play_next_level");

                            CurrentScore.Init(result.PlayedLevel, ui_score.ScoreLabel.level);
                            HighScore.Show(false);
                            ScoresContainer.SetActive(true);
                            break;
                    }
                    NextLevelButton.gameObject.SetActive(true);
                    break;
            }
            QuitButton.gameObject.SetActive(true);

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
            DisableButton(NextLevelButton);
            Close();
            var nextLevel = Statics.ActivityFlow.CurrentLevel;
            if (Statics.ActivityFlow.CurrentActivityManager.DebugPlay)
            {
                Statics.ActivityFlow.CurrentActivityManager.DebugStartLevel++;
                nextLevel = Statics.ActivityFlow.CurrentActivityManager.DebugStartLevel;
            }

            StartCoroutine(Statics.ActivityFlow.CurrentActivityManager.PlayNextLevel(nextLevel));
        }
    }
}
