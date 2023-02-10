using Ieedo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace minigame
{
    public class ui_score : MonoBehaviour
    {
        public enum ScoreLabel
        {
            DoNotChange = 0,
            score,
            hiscore,
            level,
            playcount
        }

        public ScoreLabel LabelType;

        public LocalizeStringEvent ScoreLabelLocEvent;
        public GameObject ScoreGO;
        public TextMeshProUGUI ScoreText;
        public GameObject BonusPrefab;

        private Sequence PunchAnimation;
        private GameObject bonusGO;

        void Start()
        {
            //            ScoreText.text = "";

            PunchAnimation = DOTween.Sequence()
           .SetAutoKill(false).Pause();
        }

        public void Show(bool status)
        {
            gameObject.SetActive(status);
        }

        public void Init(int score, ScoreLabel label = ScoreLabel.DoNotChange)
        {
            ScoreText.text = score.ToString();

            switch (label)
            {
                case ScoreLabel.score:
                    ScoreLabelLocEvent.StringReference = new LocalizedString("Activity", "activity_score");
                    break;
                case ScoreLabel.hiscore:
                    ScoreLabelLocEvent.StringReference = new LocalizedString("Activity", "activity_highscore");
                    break;
                case ScoreLabel.level:
                    ScoreLabelLocEvent.StringReference = new LocalizedString("Activity", "activity_level");
                    break;
                case ScoreLabel.playcount:
                    ScoreLabelLocEvent.StringReference = new LocalizedString("Activity", "activity_played");
                    break;
                case ScoreLabel.DoNotChange:
                    break;
            }
        }

        public void AddScore(int bonus, int totalScore)
        {
            if (bonus != 0)
            {
                bonusGO = Instantiate(BonusPrefab, transform);
                bonusGO.GetComponent<ui_score_bonus>().Init(bonus);
            }
            UpdateScore(totalScore);
        }

        private void UpdateScore(int value)
        {
            ScoreText.text = value.ToString();
            PunchAnimation.Rewind();
            PunchAnimation.Play();
            SoundManager.I.PlaySfx(AudioEnum.game_score);
        }
    }
}
