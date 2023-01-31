using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{
    public class InputManager : MonoBehaviour
    {
        private bool _isExecutingAction;
        public bool IsExecutingAction
        {
            get
            {
                return _isExecutingAction;
            }
            set
            {
                _isExecutingAction = value;
            }
        }

        #region Click Utilities

        private List<Action> onUpActions = new List<Action>();
        public void RegisterUpAction(Action a) => onUpActions.Add(a);
        public void UnregisterUpAction(Action a) => onUpActions.Remove(a);

        public void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                for (var index = onUpActions.Count - 1; index >= 0; index--)
                {
                    Action onUpAction = onUpActions[index];
                    onUpAction();
                }
            }
        }

        #endregion

    }
}
