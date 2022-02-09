using System;
using System.Collections;
using Lean.Gui;
using UnityEngine;

namespace Ieedo
{
    public class UIScreen : MonoBehaviour
    {
        public virtual ScreenID ID => ScreenID.None;

        public void OpenImmediate()
        {
            gameObject.SetActive(true);
        }

        public void CloseImmediate()
        {
            gameObject.SetActive(false);
        }

        public IEnumerator OpenCO()
        {
            yield return OnOpen();
            gameObject.SetActive(true);
        }

        public IEnumerator CloseCO()
        {
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