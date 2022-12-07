using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
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
                    item.CloseImmediate(true);
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
            if (!Screens.ContainsKey(id))
                yield break;
            OpenScreens.Add(id);
            yield return Screens[id].OpenCO();
        }

        public void Close(ScreenID id)
        {
            AppManager.I.StartCoroutine(CloseCO(id));
        }
        public IEnumerator CloseCO(ScreenID id)
        {
            if (!Screens.ContainsKey(id))
                yield break;
            OpenScreens.Remove(id);
            yield return Screens[id].CloseCO();
        }

        // Smooth transition from-to
        public IEnumerator CloseOpenCO(ScreenID from, ScreenID to)
        {
            Close(from);
            yield return Screens[to].OpenCO();
        }

        public void OpenImmediate(ScreenID id)
        {
            if (!Screens.ContainsKey(id))
                return;
            OpenScreens.Add(id);
            Screens[id].OpenImmediate();
        }

        public void CloseImmediate(ScreenID id)
        {
            if (!Screens.ContainsKey(id))
                return;
            OpenScreens.Remove(id);
            Screens[id].CloseImmediate();
        }

        public ScreenID CurrentFlowScreenID;

        public IEnumerator TransitionToCO(ScreenID toId)
        {
            if (CurrentFlowScreenID != ScreenID.None)
                yield return CloseCO(CurrentFlowScreenID);
            CurrentFlowScreenID = toId;
            OnSwitchToScreen?.Invoke(toId);
            yield return OpenCO(CurrentFlowScreenID);
        }

        public void GoTo(ScreenID toId)
        {
            SoundManager.I.PlaySfx(SfxEnum.click);
            var uiBottomScreen = Statics.Screens.Get(ScreenID.Bottom) as UIBottomScreen;
            LeanButton toButton = null;
            switch (toId)
            {
                case ScreenID.Pillars:
                    toButton = uiBottomScreen.btnPillars;
                    break;
                case ScreenID.CardList:
                    toButton = uiBottomScreen.btnCards;
                    break;
                case ScreenID.Activities:
                    toButton = uiBottomScreen.btnActivities;
                    break;
            }
            if (toButton != null)
                uiBottomScreen.ToggleSelection(toButton);

            Statics.I.StartCoroutine(TransitionToCO(toId));
        }

        public UIScreen Get(ScreenID id)
        {
            return Screens[id];
        }

        public event Action<ScreenID> OnSwitchToScreen;

        #region Custom Flows

        public IEnumerator ShowQuestionFlow(string titleKey, string contentKey, string[] answers, Ref<int> answer)
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            yield return questionScreen.ShowQuestionFlow(titleKey, contentKey, answers, answer);
        }

        public IEnumerator ShowDialog(string contentKey, string answerKey, bool waitForClosing = true)
        {
            var dialogScreen = Statics.Screens.Get(ScreenID.Dialog) as UIDialogPopup;
            yield return dialogScreen.ShowDialog(LocString.FromStr(contentKey), LocString.FromStr(answerKey));
            if (waitForClosing)
            {
                while (dialogScreen.IsOpen)
                {
                    yield return null;
                }
            }
        }

        public void GoToTodoList()
        {
            //StartCoroutine(GoToTodoListCO());

            SoundManager.I.PlaySfx(SfxEnum.click);
            var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
            uiCardListScreen.LoadToDoCards();
            uiCardListScreen.KeepPillars = false;
            GoTo(ScreenID.CardList);

            if (UICardListScreen.FRONT_VIEW_ONLY)
            {
                uiCardListScreen.CloseFrontView();
                if (uiCardListScreen.CardsList.HeldCards.Count > 0)
                {
                    uiCardListScreen.OpenFrontView(uiCardListScreen.CardsList.HeldCards[0], UICardListScreen.FrontViewMode.Edit);
                }
                else
                {
                    //uiCardListScreen.OpenFrontView(null, UICardListScreen.FrontViewMode.Edit);
                }
            }
        }

        /*private void StartCoroutine(object goToTodoListCO)
        {
            throw new NotImplementedException();
        }*/

        #endregion
    }
}
