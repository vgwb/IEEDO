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
        public static ActivityFastReaction I;

        void Awake()
        {
            I = this;
        }

        protected override void SetupActivity(int currentLevel)
        {
            StartGame();
        }

        void StartGame()
        {
            PlayFastReaction.I.InitGame();
        }

        public void FinishGame(int score, int level)
        {
            SoundManager.I.PlaySfx(AudioEnum.game_win);
            int points = Activity.PointsOnWin * level;
            StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, points, score, level)));
        }

        public override IEnumerator PlayNextLevel(int _currentLevel)
        {
            StartGame();
            yield break;
        }

        void Update()
        {
#if UNITY_EDITOR
            // DEBUG
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayFastReaction.I.OnBtnNo();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                PlayFastReaction.I.OnBtnYes();
            }
#endif
        }

    }
}
