using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo.games.tictac
{
    /// <summary>
    /// Simple AI. Can be beaten with "fork".
    /// </summary>
    public class AIScript : MonoBehaviour
    {

        public GameController.PlayerSymbol symbol = GameController.PlayerSymbol.O;
        public float turnDelay = 0.5f;
        public int rndCornerIterations = 5;

        private List<GameController.Int2D> prevTurns, enemyTurns;

        private void OnValidate()
        {
            turnDelay = Mathf.Max(turnDelay, 0.1f);
        }

        private void Awake()
        {
            ResetAI();
        }

        /// <summary>
        /// Makes AI to performa a turn after a cetrain delay.
        /// </summary>
        public void ActivateForTurn()
        {
            Cell lastClickedCell = GameController.controller.GetLastClickedCell();
            if (lastClickedCell != null) enemyTurns.Add(GameController.controller.GetCellMatrixIndex(lastClickedCell));

            StartCoroutine(TurnDelayCoroutine());
        }

        private IEnumerator TurnDelayCoroutine()
        {
            yield return new WaitForSeconds(turnDelay);

            PerformTurn();
        }

        /// <summary>
        /// Performs a turn. On the first rounds tries to take center and corners. Then wins if possible. If not - makes
        /// a defense move if required. If not - occupies a corner with some chance (depends on the number of iterations).
        /// If not - occupies a random cell.
        /// </summary>
        private void PerformTurn()
        {
            Cell[,] cells = GameController.controller.GetCellMatrix();
            Cell center = cells[1, 1];
            Cell[] corners = { cells[0, 0], cells[0, 2], cells[2, 0], cells[2, 2] };

            Cell targetCell;

            if (enemyTurns.Count < 2) {
                targetCell = CenterTurn(center);
                if (targetCell == null) targetCell = RandomCornerTurn(corners, 1000);
            } else {
                targetCell = SmartTurn(cells, prevTurns);

                if (targetCell == null) targetCell = SmartTurn(cells, enemyTurns);

                if (targetCell == null) {
                    targetCell = RandomCornerTurn(corners, rndCornerIterations);
                    if (targetCell == null) targetCell = RandomTurn(true);
                }
            }

            targetCell.UpdateCellState();

            prevTurns.Add(GameController.controller.GetCellMatrixIndex(targetCell));
        }

        /// <summary>
        /// Make a turn on the central cell.
        /// </summary>
        /// <param name="center">Central cell.</param>
        /// <returns>Returns targeted cell if success or null if not.</returns>
        private Cell CenterTurn(Cell center)
        {
            if (center.CurCellState == Cell.CellState.EMPTY) {
                return center;
            } else return null;
        }

        /// <summary>
        /// Make a turn on a random corner.
        /// </summary>
        /// <param name="corners">Array of corners.</param>
        /// <param name="iterations">Number of iterations.</param>
        /// <returns>Returns targeted cell if success or null if not.</returns>
        private Cell RandomCornerTurn(Cell[] corners, int iterations = 1)
        {
            for (int i = 0; i < iterations; i++) {
                Cell cor = corners[Random.Range(0, corners.Length)];

                if (cor.CurCellState == Cell.CellState.EMPTY) {
                    return cor;
                }
            }

            return null;
        }

        /// <summary>
        /// Turn, which is made on the basis of previous turns. Used for defense and attack.
        /// </summary>
        /// <param name="cells">Cells in a form of two-dimensional array.</param>
        /// <param name="turns">A list of turns. Could be enemy's for defense or AI's for attack.</param>
        /// <returns>Returns targeted cell if success or null if not.</returns>
        private Cell SmartTurn(Cell[,] cells, List<GameController.Int2D> turns)
        {
            for (int i = 0; i < turns.Count; i++) {
                GameController.Int2D outer = turns[i];

                for (int j = i + 1; j < turns.Count; j++) {
                    GameController.Int2D inner = turns[j];

                    GameController.Int2D dir = inner - outer;
                    inner += dir;
                    inner.x %= 3;
                    inner.y %= 3;
                    if (inner.x < 0) inner.x += 3;
                    if (inner.y < 0) inner.y += 3;

                    if (cells[inner.x, inner.y].CurCellState == Cell.CellState.EMPTY) {
                        return cells[inner.x, inner.y];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Just a random turn.
        /// </summary>
        /// <param name="performUntilSuccess">If true - will be executing until solution is found. BE CAREFUL, can be endless.</param>
        /// <returns>Returns targeted cell if success or null if not.</returns>
        private Cell RandomTurn(bool performUntilSuccess)
        {
            Cell[] cellList = GameController.controller.GetCellList();
            do {
                int index = Random.Range(0, cellList.Length);
                if (cellList[index].CurCellState == Cell.CellState.EMPTY) {
                    return cellList[index];
                }
            } while (performUntilSuccess);

            return null;
        }

        public void ResetAI()
        {
            prevTurns = new List<GameController.Int2D>();
            enemyTurns = new List<GameController.Int2D>();
        }
    }

}