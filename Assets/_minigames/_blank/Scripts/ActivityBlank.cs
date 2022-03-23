using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo.games.blank
{
    public class ActivityBlank : ActivityManager
    {
        protected override void SetupActivity(int currentLevel)
        {
            Debug.Log($"Starting game at level {currentLevel}");
        }

        public void OnBtnWin()
        {
            SoundManager.I.PlaySfx(SfxEnum.win);
            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, 10)));
        }

        /*
        private IEnumerator AskQuestionCO()
        {
            var answer = new Ref<int>();
            yield return ShowQuestion(new LocalizedString("UI", "continue"),
                new LocalizedString("UI", "continue"), new[]
                {
                    new LocalizedString("UI","yes"),
                    new LocalizedString("UI","no")
                }, answer);
            switch (answer.Value)
            {
                case 0:
                    Debug.Log("Game Blank Win");
                    CloseActivity(new ActivityResult(ActivityResultState.Win, 10));
                    break;
                case 1:
                    Debug.Log("Game Blank Lose");
                    CloseActivity(new ActivityResult(ActivityResultState.Lose, 2));
                    break;
            }
        }*/

        public void OnBtnLose()
        {
            SoundManager.I.PlaySfx(SfxEnum.lose);
            Debug.Log("Game Blank Lose");
            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Lose, 2)));
        }
    }
}
