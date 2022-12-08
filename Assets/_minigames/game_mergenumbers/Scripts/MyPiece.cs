namespace minigame.mergenumbers
{
    public class MyPiece : Piece
    {
        void Awake()
        {
            SetAction(Clicked);
        }

        private void Clicked(Piece piece)
        {

        }
    }
}
