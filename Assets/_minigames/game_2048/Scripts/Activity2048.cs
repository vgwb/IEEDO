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

            var points = 0;
            if (currentScore >= 2048)
            {
                points = Activity.ScoreOnWin;
            }
            else if (currentScore >= 1024)
            {
                points = Activity.ScoreOnWin / 2;
            }
            else if (currentScore >= 512)
            {
                points = Activity.ScoreOnWin / 4;
            }
            else if (currentScore >= 256)
            {
                points = Activity.ScoreOnWin / 8;
            }
            else if (currentScore >= 128)
            {
                points = Activity.ScoreOnWin / 16;
            }
            else if (currentScore >= 64)
            {
                points = Activity.ScoreOnWin / 32;
            }
            else if (currentScore >= 32)
            {
                points = Activity.ScoreOnWin / 64;
            }

            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, points)));
        }

        public void OnBtnLose()
        {
            SoundManager.I.PlaySfx(SfxEnum.lose);
            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Lose, Activity.ScoreOnLoss)));
        }

        public void OnSwipe(string direction)
        {
            SoundManager.I.PlaySfx(SfxEnum.click);
            var currentSwipe = direction switch
            {
                "N" => SwipeDirection.N,
                "E" => SwipeDirection.E,
                "S" => SwipeDirection.S,
                "W" => SwipeDirection.W,
                _ => SwipeDirection.None,
            };
            board.Swipe(currentSwipe);
        }

        public void Score(int totalScore, int deltascore = 0)
        {
            currentScore = totalScore;
            ScoreUI.UpdateScore(totalScore);
        }

    }
}

