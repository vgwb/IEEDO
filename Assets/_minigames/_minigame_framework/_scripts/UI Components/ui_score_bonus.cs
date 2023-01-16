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

        public void Init(int value)
        {
            ScoreText.text = (value >= 0 ? "+" : "-") + value;
            //            var localY = GetComponent<RectTransform>().anchoredPosition.y;
            //            GetComponent<RectTransform>().DOAnchorPosY(localY - 50, 1);
            transform.DOLocalMoveY(80, 1);
            GetComponent<CanvasGroup>().DOFade(0, 1);
            Destroy(gameObject, 1);
        }



    }
}
