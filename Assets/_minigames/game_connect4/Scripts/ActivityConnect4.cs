using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ieedo;
using Ieedo.games;

namespace minigame.connect4
{
    public class ActivityConnect4 : ActivityManager
    {
        protected override void SetupActivity(int currentLevel)
        {
            Debug.Log($"Starting game at level {currentLevel}");
        }

        public void FinishGame(bool playerWin)
        {
            if (playerWin)
            {
                OnBtnWin();
            }
            else
            {
                OnBtnLose();
            }
        }

        public void OnBtnWin()
        {
            Debug.Log("Game Blank Win");
            CloseActivity(new ActivityResult(ActivityResultState.Win, 10));
        }

        public void OnBtnLose()
        {
            Debug.Log("Game Blank Lose");
            CloseActivity(new ActivityResult(ActivityResultState.Lose, 2));
        }
    }
}
