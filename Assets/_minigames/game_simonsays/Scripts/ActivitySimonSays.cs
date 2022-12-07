using System;
using UnityEngine;
using Ieedo;
using Ieedo.games;

namespace minigame.simonsays
{
    public class ActivitySimonSays : ActivityManager
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

            Board.Setup(currentLevel);
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

        public BoardController Board;
    }
}
