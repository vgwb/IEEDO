using Ieedo;
using Ieedo.games;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace minigame.g2048
{
    public class Activity2048 : ActivityManager
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

        public void OnBtnLose()
        {
            SoundManager.I.PlaySfx(SfxEnum.lose);
            Debug.Log("Game Blank Lose");
            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Lose, 2)));
        }
    }
}
