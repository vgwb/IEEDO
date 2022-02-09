using UnityEngine;
using System.Collections;

namespace Ieedo.games
{
    public class Minigame : MonoBehaviour
    {



        void Start()
        {

        }

        public void CloseMinigame()
        {
            Debug.Log("close this minigame");
            if (AppManager.I)
            {
                AppManager.I.CloseMinigame();
            }
        }

    }
}