using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{
    public class InputManager : MonoBehaviour
    {
        #region Click Utilities

        private List<Action> onUpActions = new List<Action>();
        public void RegisterUpAction(Action a) => onUpActions.Add(a);
        public void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                foreach (Action onUpAction in onUpActions)
                    onUpAction();
            }
        }

        #endregion

    }
}
