using System;
using Lean.Gui;
using UnityEngine;

namespace Ieedo
{
    public class UIInteractable : MonoBehaviour
    {
        protected void SetupButton(LeanButton btn, Action action)
        {
            btn.OnClick.RemoveAllListeners();
            btn.interactable = true;
            btn.OnClick.AddListener(() => action());
        }

        protected void DisableButton(LeanButton btn)
        {
            btn.OnClick.RemoveAllListeners();
            btn.interactable = false;
        }

        protected void SetupButtonDown(LeanButton btn, Action downAction, Action upAction)
        {
            btn.OnDown.RemoveAllListeners();
            btn.OnDown.AddListener(() =>
            {
                Statics.Input.RegisterUpAction(upAction);
                downAction();
            });

            btn.OnClick.RemoveAllListeners();
            btn.OnClick.AddListener(() =>
            {
                upAction();
            });
        }

    }
}
