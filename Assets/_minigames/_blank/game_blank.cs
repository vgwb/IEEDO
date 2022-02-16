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
            CloseMinigame(new ActivityResult(valueContent: 1));
        }

        public void OnBtnLose()
        {
            Debug.Log("Game Blank Lose");
            CloseMinigame(new ActivityResult(valueContent: 0));
        }
    }
}