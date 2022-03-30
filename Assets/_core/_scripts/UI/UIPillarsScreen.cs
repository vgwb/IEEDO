using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Touch;
using Lean.Transition;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    public enum PillarsViewMode
    {
        NONE,
        Categories,
        Review
    }

    public class UIPillarsScreen : UIScreen
    {
        public override bool AutoAnimate => false;

        public GameObject Scene3D;
        public GameObject Plane;
        public PillarsManager PillarsManager;

        public override ScreenID ID => ScreenID.Pillars;

        public PillarsViewMode ViewMode = PillarsViewMode.Categories;

        void Awake()
        {
            var selectable = Plane.GetComponentInChildren<LeanSelectableByFinger>();
            selectable.OnSelected.AddListener(HandleSelectPlane);
        }

        public void SwitchViewMode(PillarsViewMode newMode)
        {
            ViewMode = newMode;
            StartCoroutine(OnOpen());
        }

        public void RefreshData(int onlyPillarIndex = -1)
        {
            var profileData = Statics.Data.Profile;
            var pillarsData = new PillarsData
            {
                Pillars = new List<PillarData>()
            };

            switch (ViewMode)
            {
                case PillarsViewMode.Categories:
                    foreach (var category in profileData.Categories)
                    {
                        var cards = Statics.Data.Profile.Cards.Where(x => x.Status != CardValidationStatus.Todo && x.Definition.Category == category.ID);

                        var pillarData = new PillarData
                        {
                            Color = Statics.Data.Definition(category.ID).Color,
                            Height = category.AssessmentValue,
                            Cards = cards.ToList(),
                            LocalizedKey = Statics.Data.Definition(category.ID).Title.Key
                        };

                        pillarsData.Pillars.Add(pillarData);
                    }
                    PillarsManager.SetFocus(true);
                    break;
                case PillarsViewMode.Review:
                    {
                        var completedCards = Statics.Data.Profile.Cards.Where(x => x.Status == CardValidationStatus.Completed);
                        var validatedCards = Statics.Data.Profile.Cards.Where(x => x.Status == CardValidationStatus.Validated);

                        var pillarData = new PillarData
                        {
                            Color = Color.gray,
                            Height = 0.5f,
                            Cards = validatedCards.ToList(),
                            LocalizedKey = new LocalizedString("UI","pillar_validated"),
                            IconString = "\uf560"
                        };
                        pillarsData.Pillars.Add(pillarData);

                        pillarData = new PillarData
                        {
                            Color = new Color(0.3f,0.3f,0.3f,1f),
                            Height = 0.5f,
                            Cards = completedCards.ToList(),
                            LocalizedKey = new LocalizedString("UI","pillar_completed"),
                            IconString = "\uf00c"
                        };
                        pillarsData.Pillars.Add(pillarData);

                    }
                    PillarsManager.SetFocus(false, PillarsManager.PillarViews[0]);
                    break;
            }

            AnimateToUnfocused();

            bool added = ViewMode == prevViewMode;
            prevViewMode = ViewMode;
            PillarsManager.ShowData(pillarsData, added, onlyPillarIndex);
            Scene3D.SetActive(true);

            for (var iPillar = 0; iPillar < PillarsManager.PillarViews.Count; iPillar++)
            {
                var pillarView = PillarsManager.PillarViews[iPillar];
                int _iPillar = iPillar;
                pillarView.OnSelected = () => HandleSelectPillar(pillarView, _iPillar);
            }
        }

        private PillarsViewMode prevViewMode = PillarsViewMode.NONE;
        protected override IEnumerator OnOpen()
        {
            Statics.Screens.OnSwitchToScreen -= this.OnSwitchToScreen;
            Statics.Screens.OnSwitchToScreen += this.OnSwitchToScreen;

            RefreshData();

            return base.OnOpen();
        }

        #region Interaction

        private bool isFocused;
        public void HandleSelectPillar(PillarView pillarView, int iPillar)
        {
            if (PillarsManager.CurrentFocusedPillar == pillarView) return;
            AnimateToFocused();

            PillarsManager.SetFocus(false, pillarView);

            var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;

            UICardListScreen.FrontViewMode desiredFrontViewMode = UICardListScreen.FrontViewMode.None;
            switch (Statics.Mode.SessionMode)
            {
                case SessionMode.Session:
                    desiredFrontViewMode = iPillar == 0 ? UICardListScreen.FrontViewMode.Validated : UICardListScreen.FrontViewMode.Completed;
                    break;

                case SessionMode.Solo:
                    desiredFrontViewMode = UICardListScreen.FrontViewMode.View;
                    break;
            }

            uiCardListScreen.LoadCards(pillarView.Data.Cards, UICardListScreen.SortByExpirationDate, UICardListScreen.ListViewMode.Pillars, desiredFrontViewMode);
            uiCardListScreen.KeepPillars = true;
            GoTo(ScreenID.CardList);
        }

        private void HandleSelectPlane()
        {
            if (!isFocused) return;
            AnimateToUnfocused();
            PillarsManager.SetFocus(true);

            var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
            if (uiCardListScreen.IsOpen) uiCardListScreen.Close();
        }

        #endregion

        public Camera Camera3D;
        public void OnSwitchToScreen(ScreenID screenID)
        {
            switch (screenID)
            {
                case ScreenID.Pillars:
                    AnimateToUnfocused();
                    break;
                case ScreenID.CardList:
                    var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
                    if (uiCardListScreen.KeepPillars)
                    {
                        Camera3D.transform.localRotationTransition(Quaternion.Euler(38f,10f,0f), 0.25f, LeanEase.Decelerate);
                    }
                    else
                    {
                        isFocused = false;
                        Camera3D.transform.localRotationTransition(Quaternion.Euler(38f,-50f,0f), 0.25f, LeanEase.Decelerate);
                    }
                    break;
                case ScreenID.Activities:
                    isFocused = false;
                    Camera3D.transform.localRotationTransition(Quaternion.Euler(38f,50f,0f), 0.25f, LeanEase.Decelerate);
                    break;
            }
        }

        #region Animations

        private void AnimateToFocused()
        {
            if (isFocused) return;
            isFocused = true;
            Camera3D.transform.localPositionTransition(new Vector3(0, 10.5f, -13), 0.5f);
            Camera3D.transform.localRotationTransition(new Quaternion(0.255145639f,0.0823278204f,-0.0286051836f,0.962966561f), 0.5f);
        }

        private void AnimateToUnfocused()
        {
            isFocused = false;
            Camera3D.transform.localPositionTransition(new Vector3(0,15.1000004f,-13.3999996f), 0.5f);
            Camera3D.transform.localRotationTransition(new Quaternion(0.32556805f,0,0,0.945518613f), 0.5f);
        }

        #endregion

    }
}
