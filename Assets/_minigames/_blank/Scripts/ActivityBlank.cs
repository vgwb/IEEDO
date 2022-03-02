using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            StartCoroutine(AskQuestionCO());
        }

        private IEnumerator AskQuestionCO()
        {
            var answer = new Ref<int>();
            yield return ShowQuestion("TEST POPUP", "Are you sure?", new []{"Yes", "No"}, answer);
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
        }

        public void OnBtnLose()
        {
            Debug.Log("Game Blank Lose");
            CloseActivity(new ActivityResult(ActivityResultState.Lose, 2));
        }
    }
}
