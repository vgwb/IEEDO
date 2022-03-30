using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace minigame.unblock
{
    public class Datas : ScriptableObject
    {
        private TextAsset datas;

        public string[] getData(string dataName)
        {
            datas = Resources.Load<TextAsset>(dataName + "/levels");
            string[] lines = new string[0];
            lines = datas.text.Split('\n');

            return lines;
        }
    }
}
