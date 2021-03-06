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
            if (DebugAutoplay)
            {
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
            GameManager.I().init();
            Debug.Log($"Starting game at level {currentLevel}");
            levelText.text = "Level " + (currentLevel);
            GameData.I().isLock = false;
            Unblock tg = GameObject.Find("unblock").GetComponent<Unblock>();
            tg.clear();
            GameData.instance.cLevel = currentLevel - 1;
            tg.init();
        }

        public void win()
        {
            Debug.Log("Win");
            GameData.instance.isWin = true;
            SoundManager.I.PlaySfx(SfxEnum.win);
            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, 10)));
        }

        public override IEnumerator PlayNextLevel(int _currentLevel)
        {
            currentLevel = _currentLevel;
            StartGame();
            yield break;
        }
    }
}
