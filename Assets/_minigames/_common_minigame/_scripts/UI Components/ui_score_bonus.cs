using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace minigame
{
    public class ui_score_bonus : MonoBehaviour
    {
        public TextMeshProUGUI ScoreText;
        private float currentLocalY;

        public void Init(int bonusValue)
        {
            currentLocalY = transform.localPosition.y;
            ScoreText.text = bonusValue.ToString();
            GetComponent<CanvasGroup>().DOFade(0, 1);
            if (bonusValue >= 0)
            {
                ScoreText.color = Color.yellow;
                transform.DOLocalMoveY(currentLocalY + 50, 1);
            }
            else
            {
                ScoreText.color = Color.red;
                transform.DOLocalMoveY(currentLocalY - 100, 1);
            }
            Destroy(gameObject, 3);
        }

    }
}
