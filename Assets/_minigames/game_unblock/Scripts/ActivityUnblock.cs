using System.Collections;
using UnityEngine;
using Ieedo;
using Ieedo.games;

namespace minigame.unblock
{
    public class ActivityUnblock : ActivityManager
    {
        public static ActivityUnblock I;
        public ui_score ScoreUI;

        private int currentLevel;

        void Awake()
        {
            I = this;
        }

        protected override void SetupActivity(int _currentLevel)
        {
            currentLevel = _currentLevel;
            StartGame();
        }

        void StartGame()
        {
            GameManager.I().init();
            Debug.Log($"Starting game at level {currentLevel}");
            ScoreUI.Init(currentLevel);
            GameData.I().isLock = false;
            Unblock tg = GameObject.Find("unblock").GetComponent<Unblock>();
            tg.clear();
            GameData.instance.cLevel = currentLevel - 1;
            tg.init();
        }

        public void win()
        {
            // Debug.Log("Win");
            GameData.instance.isWin = true;
            //SoundManager.I.PlaySfx(SfxEnum.game_win);
            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, Activity.PointsOnWin, 0, currentLevel)));
        }

        public override IEnumerator PlayNextLevel(int _currentLevel)
        {
            currentLevel = _currentLevel;
            StartGame();
            yield break;
        }
    }
}
