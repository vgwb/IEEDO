using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo.games.tictac
{
    /// <summary>
    /// A parent object of all cells. Initializes cells array and controls animation when appearing.
    /// </summary>
    public class FieldScript : MonoBehaviour
    {

        public float appearAnimTime = 5f;
        public float minScale = 0.05f;

        private Cell[] cells;

        private bool inAnim = false;

        private void OnValidate()
        {
            appearAnimTime = Mathf.Max(appearAnimTime, 0.1f);
        }

        private void Awake()
        {
            cells = GetComponentsInChildren<Cell>();
        }

        public void AppearAnim()
        {
            if (!inAnim) StartCoroutine(AppearCoroutine());
        }

        private IEnumerator AppearCoroutine()
        {
            float s = minScale;

            float animSpeed = (1f - minScale) / appearAnimTime;

            while (s < 1f) {
                s += animSpeed * Time.fixedDeltaTime;

                if (s > 1f) s = 1f;

                for (int i = 0; i < cells.Length; i++) {
                    cells[i].transform.localScale = new Vector3(s, s, 1f);
                }

                yield return null;
            }

            inAnim = false;
        }

        public Cell[] GetCells()
        {
            return cells;
        }
    }
}