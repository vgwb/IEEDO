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
        public static GameManager I()
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

            inited = true;
        }
    }
}
