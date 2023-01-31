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
        public GameObject BonusPrefab;

        private Sequence PunchAnimation;
        private GameObject bonusGO;

        void Start()
        {
            ScoreText.text = "";
            HiScoreText.text = "";

            PunchAnimation = DOTween.Sequence()
           .Insert(0, ScoreGO.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 1, 3, 0f))
           .SetAutoKill(false).Pause();
        }

        public void SetLevel(int level)
        {
            ScoreText.text = "Level " + level;
            HiScoreText.text = "";
        }

        public void Init(int score, int hiscore)
        {
            ScoreText.text = score.ToString();
            UpdateHiScore(hiscore);
        }

        public void AddScore(int bonus, int totalScore)
        {
            bonusGO = Instantiate(BonusPrefab, transform);
            bonusGO.GetComponent<ui_score_bonus>().Init(bonus);
            UpdateScore(totalScore);
        }

        private void UpdateScore(int value)
        {
            ScoreText.text = value.ToString();
            PunchAnimation.Rewind();
            PunchAnimation.Play();
            SoundManager.I.PlaySfx(SfxEnum.game_score);
        }
        private void UpdateHiScore(int value)
        {
            HiScoreText.text = value.ToString();
        }
    }
}
