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
            transform.localPosition = new Vector3(0, 2500, 0);
            float period = 0.25f;
            transform.localPositionTransition_y(0, period);
            yield return new WaitForSeconds(period);
            yield return OnOpen();
        }

        public IEnumerator CloseCO()
        {
            float period = 0.25f;
            transform.localPositionTransition_y(2500, period);
            yield return new WaitForSeconds(period);
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