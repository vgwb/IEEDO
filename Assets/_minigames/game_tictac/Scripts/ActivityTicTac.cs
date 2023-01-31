using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ieedo;
using Ieedo.games;

namespace minigame.tictac
{
    public class ActivityTicTac : ActivityManager
    {
        protected override void SetupActivity(int currentLevel)
        {
            Debug.Log($"Starting game at level {currentLevel}");
        }

        public void FinishGame(bool playerWin)
        {
            if (playerWin)
            {
                SoundManager.I.PlaySfx(AudioEnum.game_win);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, Activity.PointsOnWin)));
            }
            else
            {
                SoundManager.I.PlaySfx(AudioEnum.game_lose);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Lose, Activity.PointsOnLoss)));
            }
        }

    }
}
