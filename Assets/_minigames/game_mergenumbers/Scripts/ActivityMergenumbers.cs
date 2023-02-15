using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ieedo;
using Ieedo.games;
using Lean.Transition;

namespace minigame.mergenumbers
{
    public class ActivityMergenumbers : ActivityManager
    {
        protected override void SetupActivity(int currentLevel)
        {
            Debug.Log($"Starting game at level {currentLevel}");

            var gridSize = 5;
            Board.GenerateGrid(gridSize, gridSize);

            var nNumbers = gridSize * gridSize;
            var sequentialNumber = 1;
            for (int x = 0; x < Board.Grid.SizeX; x++)
            {
                for (int y = 0; y < Board.Grid.SizeY; y++)
                {
                    var cell = Board.Cell(x, y);
                    cell.Text = $"{x}-{y}";// sequentialNumber.ToString();
                    cell.Color = Color.HSVToRGB(sequentialNumber * 1f / nNumbers, 1f, 1f);
                    var _sequentialNumber = sequentialNumber;
                    cell.SetAction((c) => OnClickedCell(c, _sequentialNumber));
                    sequentialNumber++;

                    cell.MovingPart.localScale = Vector3.zero;
                }
            }

            StartCoroutine(StartGameCO());
        }

        void OnClickedCell(Cell cell, int sequentialNumber)
        {
            Debug.LogError("CLICKED cell with Number " + sequentialNumber);
        }

        private IEnumerator StartGameCO()
        {
            yield return new WaitForSeconds(1f);

            // Animate entrance of the grid
            for (int x = 0; x < Board.Grid.SizeX; x++)
            {
                for (int y = 0; y < Board.Grid.SizeY; y++)
                {
                    var cell = Board.Grid.GetCell(x, y);
                    cell.MovingPart.localScale = Vector3.zero;
                    cell.MovingPart.localScaleTransition(new Vector3(2, 2, 2), 2f, LeanEase.Accelerate)
                        .localScaleTransition(new Vector3(1, 1, 1), 0.2f, LeanEase.Accelerate);
                    yield return new WaitForSeconds(0.1f);
                }
            }

            yield return null;

            // Spawn and move pieces
            var p1 = Board.AddPiece(1, 3);
            //p1.SetAction((p) => Debug.LogError("Clicked piece " + p));
            var p2 = Board.AddPiece(4, 2);
            var p3 = Board.AddPiece(1, 1);

            yield return new WaitForSeconds(1f);
            if (Board.IsCellFree(3, 3))
            {
                Board.MovePiece(p1, 3, 3);
            }
            Board.MovePiece(p2, 3, 1);
            Board.MovePiece(p3, 2, 2);
            yield return new WaitForSeconds(1f);
            Board.RemovePiece(p1);

            var p = Board.GetPiece(3, 1);

            Debug.LogError("Piece: " + Board.GetPiece(3, 1));
        }


        public void FinishGame(bool playerWin)
        {
            if (playerWin)
            {
                SoundManager.I.PlaySfx(AudioEnum.game_win);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, 10)));
            }
            else
            {
                SoundManager.I.PlaySfx(AudioEnum.game_lose);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Lose, 10)));
            }
        }

        public BoardController Board;
    }
}
