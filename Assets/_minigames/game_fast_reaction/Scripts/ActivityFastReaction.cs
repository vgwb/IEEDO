using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ieedo;
using Ieedo.games;
using Lean.Transition;

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

            StartCoroutine(StartGameCO());
        }

        private IEnumerator StartGameCO()
        {
            yield return new WaitForSeconds(1f);

        }

        public void FinishGame(bool playerWin)
        {
            if (playerWin)
            {
                SoundManager.I.PlaySfx(SfxEnum.win);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, 10)));
            }
            else
            {
                SoundManager.I.PlaySfx(SfxEnum.lose);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Lose, 10)));
            }
        }

    }
}
