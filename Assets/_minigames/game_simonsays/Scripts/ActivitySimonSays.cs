using System;
using System.Collections;
using UnityEngine;
using Ieedo;
using Ieedo.games;
using Lean.Transition;
using minigame.tictac;
using UnityEngine.UIElements;

namespace minigame.simonsays
{
    public class ActivitySimonSays : ActivityManager
    {
        void Start()
        {
            if (DebugAutoplay)
            {
                SetupActivity(DebugStartLevel);
            }
        }

        protected override void SetupActivity(int currentLevel)
        {
            Debug.Log($"Starting game at level {currentLevel}");

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

            StartCoroutine(AnimateNumbersCO());
        }

        private IEnumerator AnimateNumbersCO()
        { for (int x = 0; x < Grid.SizeX; x++)
            {
                for (int y = 0; y < Grid.SizeY; y++)
                {
                    var cell = Grid.GetCellAt(x, y);
                    cell.MovingPart.localScale = Vector3.zero;
                    cell.MovingPart.localScaleTransition(new Vector3(2, 2, 2), 2f, LeanEase.Accelerate)
                        .localScaleTransition(new Vector3(1, 1, 1), 0.2f, LeanEase.Accelerate);
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }

        void OnClickedCell(GridCell cell, int sequentialNumber)
        {
            Debug.LogError("CLICKED cell with Number " + sequentialNumber);
        }

        public void FinishGame(bool playerWin)
        {
            if (playerWin)
            {
                SoundManager.I.PlaySfx(SfxEnum.win);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Win, 10)));
            }
            else
            {
                SoundManager.I.PlaySfx(SfxEnum.lose);
                StartCoroutine(CompleteActivity(new ActivityResult(ActivityResultState.Lose, 10)));
            }
        }

        public GridController Grid;
    }
}
