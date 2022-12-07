using System;
using Ieedo;
using UnityEngine;

namespace minigame.simonsays
{
    public class Piece : MonoBehaviour
    {
        public Cell Cell { get; set; }

        public UIButton Button;

        public void SetAction(Action<Piece> action)
        {
            Button.OnClick.RemoveAllListeners();
            Button.OnClick.AddListener(() => action(this));
        }
    }
}
