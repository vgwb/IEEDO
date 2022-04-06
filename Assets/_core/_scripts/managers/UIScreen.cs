using System;
using System.Collections;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;

namespace Ieedo
{
    public class UIScreen : MonoBehaviour
    {
        public Image BlockerBG;

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

        public void Open()
        {
            gameObject.SetActive(true);
            StartCoroutine(OpenCO());
        }

        public void Close()
        {
            StartCoroutine(CloseCO());
        }

        private Ref<Vector3> startPos;
        public IEnumerator OpenCO()
        {
            yield return OnPreOpen();
            if (BlockerBG != null)
            {
                var col = BlockerBG.color;
                col.a = 1f;
                BlockerBG.colorTransition(col, 0.25f);
            }

            gameObject.SetActive(true);
            if (AutoAnimate)
            {
                float period = 0.25f;
                if (startPos == null)
                {
                    startPos = new Ref<Vector3>();
                    startPos.Value = transform.localPosition;
                }
                transform.localPosition = DefaultOutEnterPosition;
                transform.localPositionTransition(startPos.Value, period);
                yield return new WaitForSeconds(period);
            }
            yield return OnOpen();
        }

        public IEnumerator CloseCO()
        {
            if (BlockerBG != null)
            {
                var col = BlockerBG.color;
                col.a = 0.0f;
                BlockerBG.colorTransition(col, 0.25f);
            }

            if (AutoAnimate)
            {
                float period = 0.25f;
                transform.localPositionTransition(DefaultOutExitPosition, period);
                yield return new WaitForSeconds(period);
            }
            yield return OnClose();
            gameObject.SetActive(false);
        }

        protected virtual IEnumerator OnPreOpen() { yield break; }
        protected virtual IEnumerator OnOpen() { yield break; }
        protected virtual IEnumerator OnClose() { yield break; }

        protected void SetupButton(LeanButton btn, Action action)
        {
            btn.OnClick.RemoveAllListeners();
            btn.OnClick.AddListener(() => action());
        }

        protected void SetupButtonDown(LeanButton btn, Action downAction, Action upAction)
        {
            btn.OnDown.RemoveAllListeners();
            btn.OnDown.AddListener(() => downAction());

            btn.OnClick.RemoveAllListeners();
            btn.OnClick.AddListener(() => upAction());
            Statics.Input.RegisterUpAction(upAction);
        }
    }
}
