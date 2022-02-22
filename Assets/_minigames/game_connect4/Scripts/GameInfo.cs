
namespace Ieedo.games.connect4
{
    public static class GameInfo
    {
        private static int difficulty = 0; // 0 - Easy, 1 - Medium, 2 - Hard
        private static bool selectedPlayer1 = true;

        public static void setDifficulty(int dif)
        {
            difficulty = dif;
        }

        public static void setSelectedPlayer1(bool player)
        {
            selectedPlayer1 = player;
        }

        public static int getDifficulty()
        {
            return difficulty;
        }

        public static bool getSelectedPlayer1()
        {
            return selectedPlayer1;
        }


    }
}