using UnityEngine;

namespace minigame.sequence
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
