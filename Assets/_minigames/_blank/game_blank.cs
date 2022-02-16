using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ieedo.games
{
    public class game_blank : ActivityLogic
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
            CloseActivity(new ActivityResult(ActivityResultState.Win, 10));
        }

        public void OnBtnLose()
        {
            Debug.Log("Game Blank Lose");
            CloseActivity(new ActivityResult(ActivityResultState.Lose, 2));
        }
    }
}