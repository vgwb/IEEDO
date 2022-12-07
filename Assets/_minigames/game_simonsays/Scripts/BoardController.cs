using System.Collections;
using System.Collections.Generic;
using Lean.Transition;
using UnityEngine;

namespace minigame.simonsays
{
    public class BoardController : MonoBehaviour
    {
        public GridController Grid;

        public Piece PiecePrefab;


        #region Pieces

        private List<Piece> pieces = new List<Piece>();

        public Piece[] AllPieces => pieces.ToArray();

        public Piece AddPiece(int x, int y)
        {
            var piece = Instantiate(PiecePrefab, Grid.transform, true);
            piece.transform.localScale = Vector3.one;
            pieces.Add(piece);

            PlacePiece(piece, x, y);
            return piece;
        }

        private void PlacePiece(Piece piece, int x, int y)
        {
            var cell = Cell(x, y);
            piece.Cell = cell;
            cell.Piece = piece;
            piece.transform.position = Cell(x, y).transform.position;
        }

        public void RemovePiece(Piece piece)
        {
            piece.Cell.Piece = null;
            piece.Cell = null;
            Destroy(piece);
            pieces.Remove(piece);
        }

        public void MovePiece(Piece piece, int toX, int toY)
        {
            var pastCell = piece.Cell;
            piece.Cell.Piece = null;
            PlacePiece(piece, toX, toY);

            piece.transform.position = pastCell.transform.position;
            piece.transform.positionTransition(piece.Cell.transform.position, 0.5f, LeanEase.Elastic);
        }

        public bool IsCellOccupied(int x, int y)
        {
            return Cell(x, y).Piece != null;
        }

        public bool IsCellFree(int x, int y)
        {
            return Cell(x, y).Piece == null;
        }

        public Cell Cell(int x, int y)
        {
            return Grid.GetCell(x, y);
        }

        public T Cell<T>(int x, int y) where T : MonoBehaviour
        {
            return Grid.GetCell(x, y).GetComponent<T>();
        }

        public Piece GetPiece(int x, int y)
        {
            return Grid.GetCell(x, y).Piece;
        }

        #endregion

        public void GenerateGrid(int x, int y)
        {
            Grid.Regenerate(x, y);
        }
    }
}
