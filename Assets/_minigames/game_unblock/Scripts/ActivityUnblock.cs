using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Ieedo;
using Ieedo.games;

namespace minigame.unblock
{
    public class ActivityUnblock : ActivityManager
    {
        public static ActivityUnblock I;
        public TMPro.TextMeshProUGUI levelText;
        private int currentLevel;
        void Awake()
        {
            I = this;
        }

        void Start()
        {
            if (DebugAutoplay) {
                SetupActivity(DebugStartLevel);
            }
        }

        protected override void SetupActivity(int _currentLevel)
        {
            Inited = true;
            currentLevel = _currentLevel;
            StartGame();
        }

        void StartGame()
        {
            GameManager.getInstance().init();
            Debug.Log($"Starting game at level {currentLevel}");
            levelText.text = "Level " + (currentLevel + 1);

            GameData.getInstance().isLock = false;

            Unblock tg = GameObject.Find("unblock").GetComponent<Unblock>();
            tg.clear();

            GameData.difficulty = 0;
            GameData.instance.cLevel = currentLevel; //Random.Range(0, GameData.totalLevel[GameData.difficulty]);

            tg.init();

        }

        public void FinishGame(bool playerWin)
        {
            if (playerWin) {
                OnBtnWin();
            } else {
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
            SoundManager.I.PlaySfx(SfxEnum.win);
            StartCoroutine(AskQuestionCO());
        }

        private IEnumerator AskQuestionCO()
        {
            var answer = new Ref<int>();
            yield return ShowQuestion(new LocalizedString("UI", "continue"),
                new LocalizedString("UI", "continue"), new[]
                {
                    new LocalizedString("UI","yes"),
                    new LocalizedString("UI","no")
                }, answer);
            switch (answer.Value) {
                case 0:
                    currentLevel++;
                    StartGame();
                    break;
                case 1:
                    Debug.Log("Game Blank Exit");
                    CloseActivity(new ActivityResult(ActivityResultState.Win, 10));
                    break;
            }
        }
    }
}
