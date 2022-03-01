using System.Collections;
using System.Collections.Generic;

namespace minigame.unblock
{
    public class LevelEntity
    {
        public LevelEntity()
        {
            pieces = new List<Piece>();
        }

        public int minMoves;
        public List<Piece> pieces;
    }

    public class Piece
    {
        public int x = 0;
        public int y = 0;
        public int pType = 0;
        public int _w;
        public int _h;
        public int _x;
        public int _y;
    }
}
