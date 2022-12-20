using Ieedo;
using Ieedo.games;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace minigame.g2048
{
    public enum SwipeDirection
    {
        None = 0,
        N = 1,
        E = 2,
        S = 3,
        W = 4
    }

    public class Activity2048 : ActivityManager
    {
        public Board board;

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

        public void OnSwipe(string direction)
        {
            SoundManager.I.PlaySfx(SfxEnum.win);
            SwipeDirection currentSwipe = SwipeDirection.None;
            switch (direction)
            {
                case "N":
                    currentSwipe = SwipeDirection.N;
                    break;
                case "E":
                    currentSwipe = SwipeDirection.E;
                    break;
                case "S":
                    currentSwipe = SwipeDirection.S;
                    break;
                case "W":
                    currentSwipe = SwipeDirection.W;
                    break;
                default:
                    currentSwipe = SwipeDirection.None;
                    break;
            }
            board.Swipe(currentSwipe);
        }

    }
}
