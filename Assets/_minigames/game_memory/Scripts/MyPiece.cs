using Ieedo.games;
using UnityEngine;

namespace Ieedo.games.memory
{
    public class MyPiece : Piece
    {
        void Awake()
        {
            SetAction(Clicked);
        }

        private void Clicked(Piece piece)
        {
            Debug.Log("CLICKED piece with Number " + piece.Cell.X + " - " + piece.Cell.Y);
        }
    }
}
