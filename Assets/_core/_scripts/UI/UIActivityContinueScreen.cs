using System.Collections;
using System.Collections.Generic;
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

        public IEnumerator ShowResult(ActivityResult result)
        {
            //Title.Text.Key = new LocalizedString("UI", "activity_result_title");

            switch (Statics.ActivityFlow.CurrentActivity.Type)
            {
                case ActivityType.Daily:
                    Title.Text.Key = new LocalizedString("UI", $"activity_result_daily_complete");
                    Content.Text.Key =  new LocalizedString("UI", "activity_result_write_everyday");
                    break;
                case ActivityType.Game:
                    Title.Text.Key = new LocalizedString("UI", $"activity_result_game_{result.Result.ToString().ToLower()}");
                    Content.Text.Key =  new LocalizedString("UI", "activity_result_play_everyday");
                    break;
            }

            NextLevelButton.gameObject.SetActive(Statics.ActivityFlow.CurrentActivity.Type == ActivityType.Game);

            SetupButton(NextLevelButton, Continue);
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
