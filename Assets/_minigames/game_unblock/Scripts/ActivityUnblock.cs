using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ieedo;
using Ieedo.games;

namespace minigame.unblock
{
    public class ActivityUnblock : ActivityManager
    {
        public static ActivityUnblock I;
        private int currentLevel;
        void Awake()
        {
            I = this;
        }

        void Start()
        {
            currentLevel = 1;
            StartGame();
        }

        protected override void SetupActivity(int _currentLevel)
        {
            currentLevel = _currentLevel;
            StartGame();
        }

        void StartGame()
        {
            GameManager.getInstance().init();
            Debug.Log($"Starting game at level {currentLevel}");

            GameData.getInstance().isLock = false;

            Unblock tg = GameObject.Find("unblock").GetComponent<Unblock>();
            tg.clear();

            GameData.difficulty = 0;
            GameData.instance.cLevel = currentLevel; //Random.Range(0, GameData.totalLevel[GameData.difficulty]);

            tg.init();

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

        public void LevelCompletesd()
        {
            //     if (GameData.getInstance().cLevel < GameData.totalLevel[GameData.difficulty] - 1)
            // {
            //     GameData.getInstance().cLevel++;
            // }
            // else
            // {
            //     GameData.getInstance().cLevel = 0;
            // }
        }

        public void win()
        {
            Debug.Log("WIN");
            GameData.instance.isWin = true;
        }

    }
}
