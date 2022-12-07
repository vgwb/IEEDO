using System;
using Ieedo;
using Lean.Transition;
using UnityEngine;

namespace minigame
{
    public class GridCell : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public void Setup(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public string Text
        {
            set => Button.Text = value;
        }
        public Color Color
        {
            set => Button.SetColor(value);
        }


        public UIButton Button;

        public RectTransform MovingPart;

        public void SetAction(Action<GridCell> action)
        {
            Button.OnClick.RemoveAllListeners();
            Button.OnClick.AddListener(() => action(this));
        }
    }
}
