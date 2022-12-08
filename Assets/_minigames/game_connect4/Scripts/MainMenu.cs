using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace minigame.connect4
{
    public class MainMenu : MonoBehaviour
    {
        public Button easy;
        public Button medium;
        public Button hard;

        public void playButton(bool player1)
        {
            GameInfo.setSelectedPlayer1(player1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void quitButton()
        {
            Debug.Log("Quit");
            Application.Quit();
        }

        public void prepareDifficultyMenu()
        {
            switch (GameInfo.getDifficulty())
            {
                case 0:
                    easy.Select();
                    break;
                case 1:
                    medium.Select();
                    break;
                case 2:
                    hard.Select();
                    break;
                default:
                    easy.Select();
                    break;
            }
        }

        public void setDifficulty(int dif)
        {
            Debug.Log("Difficulty set to: " + dif);
            if (dif > 2 || dif < 0)
                GameInfo.setDifficulty(0);
            else
                GameInfo.setDifficulty(dif);
        }
    }
}
