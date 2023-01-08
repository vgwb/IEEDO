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
        public ui_score ScoreUI;
        public static Activity2048 I;

        private int currentScore;

        void Awake()
        {
            I = this;
            board.CreateBoard();
        }

        void Start()
        {
            if (DebugAutoplay)
            {
                SetupActivity(DebugStartLevel);
            }
        }
        protected override void SetupActivity(int currentLevel)
        {
            int maxScore = 0;
            if (Statics.Data != null)
            {
                var activityData = Statics.Data.Profile?.Activities?.GetActivityData(ActivityID.Play_2048);
                if (activityData != null)
                {
                    Debug.Log(activityData.MaxScore);
                    maxScore = activityData.MaxScore;
                }
            }
            Debug.Log($"Starting game at level {currentLevel}");

            ScoreUI.Init(0, maxScore);
            board.StartGame();
        }

        public override IEnumerator PlayNextLevel(int _currentLevel)
        {
            Debug.Log("NEXT GAME!");
            board.Reset();
            board.CreateBoard();
            board.StartGame();
            yield break;
        }

        public void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnBtnWin();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                board.StartGame();
            }
#endif
        }

        public void OnBtnWin()
        {
            SoundManager.I.PlaySfx(SfxEnum.win);
            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, currentScore)));
        }

        public void OnBtnLose()
        {
            SoundManager.I.PlaySfx(SfxEnum.lose);
            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Lose, currentScore)));
        }

        public void OnSwipe(string direction)
        {
            SoundManager.I.PlaySfx(SfxEnum.click);
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

        public void Score(int totalScore, int deltascore = 0)
        {
            currentScore = totalScore;
            ScoreUI.UpdateScore(totalScore);
        }

    }
}

