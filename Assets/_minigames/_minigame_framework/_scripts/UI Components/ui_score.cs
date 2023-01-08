using Ieedo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace minigame
{
    public class ui_score : MonoBehaviour
    {
        public GameObject ScoreGO;
        public TextMeshProUGUI ScoreText;
        public TextMeshProUGUI HiScoreText;

        private Sequence PunchAnimation;

        void Start()
        {
            ScoreText.text = "";
            HiScoreText.text = "";

            PunchAnimation = DOTween.Sequence()
           .Insert(0, ScoreGO.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 1, 3, 0f))
           .SetAutoKill(false).Pause();
        }

        public void Init(int score, int hiscore)
        {
            ScoreText.text = score.ToString();
            UpdateHiScore(hiscore);
        }

        public void UpdateScore(int value)
        {
            ScoreText.text = value.ToString();
            PunchAnimation.Rewind();
            PunchAnimation.Play();
            SoundManager.I.PlaySfx(SfxEnum.score);
        }
        public void UpdateHiScore(int value)
        {
            HiScoreText.text = value.ToString();
        }
    }
}
