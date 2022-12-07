using System.Collections;
using Lean.Transition;
using UnityEngine;

namespace minigame.simonsays
{
    public class BoardController : MonoBehaviour
    {
        public GridController Grid;

        public Piece PiecePrefab;

        public void Setup(int currentLevel)
        {
            var gridSize = currentLevel + 1;
            Grid.Regenerate(gridSize, gridSize);

            var nNumbers = gridSize * gridSize;
            var sequentialNumber = 1;
            for (int x = 0; x < Grid.SizeX; x++)
            {
                for (int y = 0; y < Grid.SizeY; y++)
                {
                    var cell = Grid.GetCellAt(x, y);
                    cell.Text = sequentialNumber.ToString();
                    cell.Color = Color.HSVToRGB(sequentialNumber * 1f/nNumbers, 1f, 1f);
                    var _sequentialNumber = sequentialNumber;
                    cell.SetAction((c) => OnClickedCell(c, _sequentialNumber));
                    sequentialNumber++;

                    cell.MovingPart.localScale = Vector3.zero;
                }
            }

            StartCoroutine(StartGameCO());
        }

        private IEnumerator StartGameCO()
        {
            // Animate entrance of the grid
            for (int x = 0; x < Grid.SizeX; x++)
            {
                for (int y = 0; y < Grid.SizeY; y++)
                {
                    var cell = Grid.GetCellAt(x, y);
                    cell.MovingPart.localScale = Vector3.zero;
                    cell.MovingPart.localScaleTransition(new Vector3(2, 2, 2), 2f, LeanEase.Accelerate)
                        .localScaleTransition(new Vector3(1, 1, 1), 0.2f, LeanEase.Accelerate);
                    //yield return new WaitForSeconds(0.1f);
                }
            }

            yield return null;

            // Spawn the piece
            var piece = Instantiate(PiecePrefab);
            piece.transform.SetParent(Grid.transform);
            piece.transform.localScale = Vector3.one;

            piece.transform.position = Grid.GetCellAt(1, 3).transform.position;
            piece.transform.positionTransition(Grid.GetCellAt(4, 2).transform.position, 3f, LeanEase.Elastic);
        }

        void OnClickedCell(GridCell cell, int sequentialNumber)
        {
            Debug.LogError("CLICKED cell with Number " + sequentialNumber);
        }

    }
}
