using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace minigame
{
    [ExecuteInEditMode]
    public class GridController : MonoBehaviour
    {
        [Range(1, 20)]
        public int Rows = 4;
        [Range(1, 20)]
        public int Columns = 3;

        public int SizeX => Columns;
        public int SizeY => Rows;

        private static int Max = 20;

        [Range(100, 500)]
        public int Span = 200;

        public Cell CellPrefab;

        public List<Cell> pool;
        private List<Cell> cells;   // TODO: the current list of cells

        private bool cleanupAndRegenerate;

        public void OnValidate()
        {
            if (CellPrefab == null)
                return;
            Regenerate(Rows, Columns);

            SetAction((cell) =>
            {
                Debug.LogError("CLICKED " + cell.name);
            });
        }

        public Cell GetCell(int x, int y)
        {
            return cells[x * Columns + y];
        }

        public void SetAction(Action<Cell> action)
        {
            foreach (Cell gridCell in pool)
            {
                gridCell.SetAction(action);
            }
        }

        public void Regenerate(int rows, int columns)
        {
            if (pool == null)
                pool = new List<Cell>();

            var currLength = pool.Count;
            for (int i = currLength; i < Max * Max; i++)
            {
                var prefabInstance = PrefabUtility.InstantiatePrefab(CellPrefab) as Cell;
                prefabInstance.transform.SetParent(transform);
                prefabInstance.transform.localScale = Vector3.one;
                pool.Add(prefabInstance);
            }

            Rows = rows;
            Columns = columns;

            if (cells == null)
                cells = new List<Cell>();
            cells.Clear();
            var index = 0;
            for (int iRow = 0; iRow < rows; iRow++)
            {
                for (int iCol = 0; iCol < columns; iCol++)
                {
                    Cell cell;
                    cell = pool[index];

                    var iX = iCol - columns / 2f + 0.5f;
                    var iY = iRow - rows / 2f + 0.5f;

                    cell.name = $"Cell_{iCol}_{iRow}";
                    cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(iX, iY) * Span;
                    cell.gameObject.SetActive(true);
                    index++;
                    cells.Add(cell);
                }
            }

            for (int i = index; i < pool.Count; i++)
            {
                pool[i].name = "Cell_unused";
                pool[i].gameObject.SetActive(false);
            }
        }
    }
}
