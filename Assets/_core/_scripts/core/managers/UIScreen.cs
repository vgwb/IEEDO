﻿using System;
using System.Collections;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;

namespace Ieedo
{
    public class UIScreen : MonoBehaviour
    {
        public virtual ScreenID ID => ScreenID.None;
        public virtual bool AutoAnimate => true;

        public Vector3 DefaultOutEnterPosition = new(-2500, 0, 0);
        public Vector3 DefaultOutExitPosition = new(2500, 0, 0);

        public bool IsOpen => gameObject.activeSelf;

        public void OpenImmediate()
        {
            gameObject.SetActive(true);
        }

        public void CloseImmediate()
        {
            gameObject.SetActive(false);
        }

        public void Close()
        {
            StartCoroutine(CloseCO());
        }

        public IEnumerator OpenCO()
        {
            gameObject.SetActive(true);
            if (AutoAnimate)
            {
                transform.localPosition = DefaultOutEnterPosition;
                float period = 0.25f;
                transform.localPositionTransition(Vector3.zero, period);
                yield return new WaitForSeconds(period);
            }
            yield return OnOpen();
        }

        public IEnumerator CloseCO()
        {
            if (AutoAnimate)
            {
                float period = 0.25f;
                transform.localPositionTransition(DefaultOutExitPosition, period);
                yield return new WaitForSeconds(period);
            }
            yield return OnClose();
            gameObject.SetActive(false);
        }

        protected virtual IEnumerator OnOpen() { yield break; }
        protected virtual IEnumerator OnClose() { yield break; }

        public void GoTo(ScreenID toId)
        {
            Statics.Screens.GoTo(toId);
        }

        protected void SetupButton(LeanButton btn, Action action)
        {
            btn.OnClick.RemoveAllListeners();
            btn.OnClick.AddListener(() => action());
        }
    }
}