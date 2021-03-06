using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ieedo;
using Ieedo.games;

namespace minigame.connect4
{
    public class Inputs : MonoBehaviour
    {
        public int column;
        public GameManager gm;

        private void OnMouseDown()
        {
            gm.selectColumn(column);
        }

        private void OnMouseOver()
        {
            gm.hoverColumn(column);
        }
    }
}
