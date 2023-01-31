using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ieedo;
using Ieedo.games;

namespace minigame.fast_reaction
{
    public class ActivityFastReaction : ActivityManager
    {

        void Start()
        {
            if (DebugAutoplay)
            {
                SetupActivity(DebugStartLevel);
            }
        }

        protected override void SetupActivity(int currentLevel)
        {
            Debug.Log($"Starting game at level {currentLevel}");

            PlayFastReaction.I.InitGame();
        }

        public void FinishGame(bool playerWin)
        {
            if (playerWin)
            {
                SoundManager.I.PlaySfx(AudioEnum.game_win);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, 10)));
            }
            else
            {
                SoundManager.I.PlaySfx(AudioEnum.game_lose);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Lose, 10)));
            }
        }

    }
}
