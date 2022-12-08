using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace minigame.tictac
{
    public class Cell : MonoBehaviour
    {

        public float fillAnimTime = 1f;

        private CellState curCellState = CellState.EMPTY;
        private Image X, O;

        public enum CellState { X, O, EMPTY }

        public CellState CurCellState
        {
            get { return curCellState; }
        }

        private void OnValidate()
        {
            fillAnimTime = Mathf.Max(fillAnimTime, 0.1f);
        }

        // Use this for initialization
        void Start()
        {
            X = transform.Find("X").GetComponent<Image>();
            O = transform.Find("O").GetComponent<Image>();

            X.color = GameController.controller.xColor;
            O.color = GameController.controller.oColor;

            ResetCell();
        }

        /// <summary>
        /// Sets cell's state. Is used by players and AI to make turns.
        /// </summary>
        public void UpdateCellState()
        {
            if (CurCellState == CellState.EMPTY)
            {
                Image fillingImage;

                if (GameController.controller.CurPlayerSymbol == GameController.PlayerSymbol.X)
                {
                    curCellState = CellState.X;
                    fillingImage = X;
                }
                else
                {
                    curCellState = CellState.O;
                    fillingImage = O;
                }

                GameController.controller.UpdateTurn(this);

                StartCoroutine(FillCellAnim(fillingImage));
            }
        }

        private IEnumerator FillCellAnim(Image fillingImage)
        {
            fillingImage.gameObject.SetActive(true);
            fillingImage.fillAmount = 0f;

            float fillAnimSpeed = 1f / fillAnimTime;

            while (fillingImage.fillAmount < 1f)
            {
                fillingImage.fillAmount += fillAnimSpeed * Time.fixedDeltaTime;

                fillingImage.fillAmount = Mathf.Min(fillingImage.fillAmount, 1f);

                yield return null;
            }
        }

        public void ResetCell()
        {
            X.gameObject.SetActive(false);
            O.gameObject.SetActive(false);

            X.fillAmount = 0f;
            O.fillAmount = 0f;

            curCellState = CellState.EMPTY;
        }
    }
}
