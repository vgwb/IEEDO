using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ieedo.games
{
    public class game_blank : Minigame
    {
        void Start()
        {

        }

        void Update()
        {

        }

        public void OnBtnWin()
        {
            Debug.Log("Game Blank Win");

        }

        public void OnBtnLose()
        {
            Debug.Log("Game Blank Lose");
            CloseMinigame();
        }
    }
}