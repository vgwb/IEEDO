using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ieedo;
using Ieedo.games;

namespace minigame.tictac
{
    public class WinLineScript : MonoBehaviour
    {
        public float animTime, maxWidthStraight, maxWidthDiag;
        private float startWidth, maxWidth;
        private RectTransform rectTransform;

        private void OnValidate()
        {
            animTime = Mathf.Max(animTime, 0.1f);
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            startWidth = rectTransform.sizeDelta.x;
            maxWidth = maxWidthStraight;
        }

        private void OnEnable()
        {
            StartCoroutine(ScaleCoroutine());
        }

        private IEnumerator ScaleCoroutine()
        {
            while (rectTransform.sizeDelta.x < maxWidth)
            {
                float scaleSpeed = (maxWidth - startWidth) / animTime;

                rectTransform.sizeDelta += new Vector2(scaleSpeed * Time.deltaTime, 0f);

                if (rectTransform.sizeDelta.x > maxWidth)
                    rectTransform.sizeDelta = new Vector2(maxWidth, rectTransform.sizeDelta.y);

                yield return null;
            }
        }

        /// <summary>
        /// Changes maxWidth of the line for straight or diagonal conditions.
        /// </summary>
        /// <param name="straight">If true - line is straight, if false - line is diagonal.</param>
        public void SetStraight(bool straight)
        {
            if (straight)
                maxWidth = maxWidthStraight;
            else
                maxWidth = maxWidthDiag;
        }

        public void ResetLine()
        {
            rectTransform.sizeDelta = new Vector2(startWidth, rectTransform.sizeDelta.y);
        }
    }
}
