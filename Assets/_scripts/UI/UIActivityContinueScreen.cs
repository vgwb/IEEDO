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
                    Title.Text.Key = new LocalizedString("Activity", $"activity_result_daily_complete");
                    //Content.Text.Key = new LocalizedString("Activity", $"{activityDef.LocName}_end");
                    Content.Text.Key = new LocalizedString("Activity", $"daily_write_diary_end");
                    NextLevelButton.gameObject.SetActive(false);
                    ScorePivot.SetActive(false);
                    break;
                case ActivityType.Game:
                    switch (activityDef.ScoreType)
                    {
                        case ScoreType.Highscore:
                            Title.Text.Key = new LocalizedString("Activity", "activity_result_game_over");
                            Content.Text.Key = new LocalizedString("Activity", "activity_play_everyday");
                            NextLevelButton.Key = new LocalizedString("Activity", "activity_play_again");

                            var scoreLoc = new LocalizedString("Activity", $"activity_score").GetLocalizedString();
                            CurrentScore.Text.SetTextRaw(scoreLoc + ": " + result.Score);
                            var highscoreLoc = new LocalizedString("Activity", $"activity_highscore").GetLocalizedString();
                            HighScore.Text.SetTextRaw(highscoreLoc + ": " + activityData.HiScore);
                            ScorePivot.SetActive(true);
                            break;
                        case ScoreType.NumberOfPlays:
                            Title.Text.Key = new LocalizedString("Activity", $"activity_result_game_{result.Result.ToString().ToLower()}");
                            Content.Text.Key = new LocalizedString("Activity", "activity_play_everyday");
                            NextLevelButton.Key = new LocalizedString("Activity", "activity_play_again");
                            ScorePivot.SetActive(false);
                            break;
                        case ScoreType.LevelReached:
                            Title.Text.Key = new LocalizedString("Activity", $"activity_completed");
                            Content.Text.Key = new LocalizedString("Activity", "activity_play_everyday");
                            NextLevelButton.Key = new LocalizedString("Activity", "activity_play_next_level");
                            var levelLoc = new LocalizedString("Activity", $"activity_level").GetLocalizedString();
                            CurrentScore.Text.SetTextRaw(levelLoc + ": " + result.PlayedLevel);
                            HighScore.Text.SetTextRaw("");
                            ScorePivot.SetActive(true);
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
            StartCoroutine(Statics.ActivityFlow.CurrentActivityManager.PlayNextLevel(Statics.ActivityFlow.CurrentLevel));
        }
    }
}
