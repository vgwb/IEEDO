using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo.games.diary
{
    public class ActivityDiary : ActivityManager
    {
        protected override void SetupActivity(int currentLevel)
        {
            Debug.Log($"Starting ActivityDiary");

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
