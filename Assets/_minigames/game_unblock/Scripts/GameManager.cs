using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace minigame.unblock
{
    public class GameManager
    {
        public static bool inited;

        public GameObject getObjectByName(string objname)
        {
            GameObject rtnObj = null;
            foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.name == objname)
                {
                    rtnObj = obj;
                }
            }
            return rtnObj;
        }

        public static GameManager instance;
        public static GameManager getInstance()
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }


        public void init()
        {
            if (inited)
                return;

            int allScore = 0;

            GameData.getInstance().levelStates = new List<List<int>>();
            for (int i = 0; i < GameData.totalLevel.Length; i++)
            {
                GameData.instance.levelStates.Add(new List<int>());
                for (int j = 0; j < GameData.totalLevel[i]; j++)
                {

                    int tState = PlayerPrefs.GetInt("blockout_" + i + "_" + j, 0);
                    GameData.instance.levelStates[i].Add(tState);
                    GameData.getInstance().levelStates[i][j] = tState;

                    if (tState == 1)
                    {
                        allScore++;
                    }
                }
            }

            GameData.instance.levelPass = new List<int>();
            for (int i = 0; i < GameData.totalLevel.Length; i++)
            {
                int tDiffLevelPassed = PlayerPrefs.GetInt("levelPassed" + i);
                GameData.instance.levelPass.Add(tDiffLevelPassed);
            }
            GameData.instance.bestScore = allScore;
            GameData.getInstance().bestScore = allScore;
            inited = true;
        }

    }
}
