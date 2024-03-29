﻿using System.Linq;
using Lean.Gui;
using Lean.Transition;
using Lean.Transition.Method;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Ieedo
{
    public class UIButton : LeanButton
    {
        public string Text
        {
            set
            {
                var tm = GetComponentInChildren<UIText>(true);
                tm.text = value;
            }
        }

        public LocalizedString Key
        {
            set
            {
                var tm = GetComponentInChildren<UIText>(true);
                tm.Key = value;
            }
        }

        public Sprite Sprite
        {
            set
            {
                var tm = GetComponentInChildren<Image>(false);
                tm.sprite = value;
            }
        }

        public Image Shadow;
        public Image Cap;
        public LeanGraphicColor NormalColor;
        public LeanGraphicColor DownColor;

        public void SetColor(Color c)
        {
            Cap.color = c;
            NormalColor.Data.Value = c;
            DownColor.Data.Value = new Color(c.r, c.g, c.b, 1f);
        }

        public void SetTextColor(Color c)
        {
            var tm = GetComponentInChildren<UIText>(true);
            tm.color = c;
        }

        public void AnimateAppear()
        {
            transform.localScaleTransition(Vector3.one, 0.25f);
        }

        public void AnimateDisappear()
        {
            transform.localScaleTransition(Vector3.zero, 0.25f);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
