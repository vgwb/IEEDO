using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{
    /// <summary>
    /// Handles the flow of the application and its scenes.
    /// </summary>
    public class ScreensManager
    {
        private Dictionary<ScreenID, UIScreen> Screens = new();

        public void LoadScreens()
        {
            var canvases = GameObject.FindObjectsOfType<Canvas>();
            foreach (var canvas in canvases)
            {
                var items = canvas.GetComponentsInChildren<UIScreen>(true);
                foreach (var item in items)
                {
                    Screens[item.ID] = item;
                    item.CloseImmediate();
                }
            }
        }

        public List<ScreenID> ScreensStack = new();
        public List<ScreenID> OpenScreens = new();

        public void Open(ScreenID id)
        {
            AppManager.I.StartCoroutine(OpenCO(id));
        }
        public IEnumerator OpenCO(ScreenID id)
        {
            if (!Screens.ContainsKey(id)) yield break;
            OpenScreens.Add(id);
            yield return Screens[id].OpenCO();
        }

        public void Close(ScreenID id)
        {
            AppManager.I.StartCoroutine(CloseCO(id));
        }
        public IEnumerator CloseCO(ScreenID id)
        {
            if (!Screens.ContainsKey(id)) yield break;
            OpenScreens.Remove(id);
            yield return Screens[id].CloseCO();
        }

        public void OpenImmediate(ScreenID id)
        {
            if (!Screens.ContainsKey(id)) return;
            OpenScreens.Add(id);
            Screens[id].OpenImmediate();
        }

        public void CloseImmediate(ScreenID id)
        {
            if (!Screens.ContainsKey(id)) return;
            OpenScreens.Remove(id);
            Screens[id].CloseImmediate();
        }

        public ScreenID CurrentFlowScreenID;

        public IEnumerator TransitionToCO(ScreenID toId)
        {
            if (CurrentFlowScreenID != ScreenID.None) yield return CloseCO(CurrentFlowScreenID);
            CurrentFlowScreenID = toId;
            OnSwitchToScreen?.Invoke(toId);
            yield return OpenCO(CurrentFlowScreenID);
        }

        public void GoTo(ScreenID toId)
        {
            Statics.I.StartCoroutine(TransitionToCO(toId));
        }

        public UIScreen Get(ScreenID id)
        {
            return Screens[id];
        }

        public event Action<ScreenID> OnSwitchToScreen;
    }
}