using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        public UIButton SwitchViewButton;

        [Header("Tutorial")]
        public UITextContent PillarsTutorial;

        public override ScreenID ID => ScreenID.Pillars;

        public PillarsViewMode ViewMode = PillarsViewMode.Categories;

        void Awake()
        {
            var selectable = Plane.GetComponentInChildren<LeanSelectableByFinger>();
            selectable.OnSelected.AddListener(HandleSelectPlane);

            SetupButton(SwitchViewButton, ToggleViewMode);
        }


        public void ToggleViewMode()
        {
            SwitchViewMode(ViewMode == PillarsViewMode.Categories ? PillarsViewMode.Review : PillarsViewMode.Categories);
        }

        public void SwitchViewMode(PillarsViewMode newMode)
        {
            StartCoroutine(CloseCardList());

            ViewMode = newMode;
            StartCoroutine(OnOpen());
        }

        public PillarsData RegenerateData()
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
                        var cards = Statics.Data.Profile.Cards.Where(x => x.Status != CardStatus.Todo && x.Definition.Category == category.ID);

                        var pillarData = new PillarData
                        {
                            Color = Statics.Data.Definition(category.ID).Color,
                            Height = category.AssessmentValue,
                            Cards = cards.ToList(),
                            LocalizedKey = Statics.Data.Definition(category.ID).Title.Key
                        };

                        pillarsData.Pillars.Add(pillarData);
                    }
                    pillarsData.ReviewMode = false;
                    break;
                case PillarsViewMode.Review:
                {
                    var completedCards = Statics.Data.Profile.Cards.Where(x => x.Status == CardStatus.Completed);
                    var validatedCards = Statics.Data.Profile.Cards.Where(x => x.Status == CardStatus.Validated);

                    var pillarData = new PillarData
                    {
                        Color = Color.gray,
                        Height = 0.5f,
                        Cards = validatedCards.ToList(),
                        LocalizedKey = new LocalizedString("UI", "pillar_validated"),
                        IconString = "\uf560"
                    };
                    pillarsData.Pillars.Add(pillarData);

                    pillarData = new PillarData
                    {
                        Color = new Color(0.3f, 0.3f, 0.3f, 1f),
                        Height = 0.5f,
                        Cards = completedCards.ToList(),
                        LocalizedKey = new LocalizedString("UI", "pillar_completed"),
                        IconString = "\uf00c"
                    };
                    pillarsData.Pillars.Add(pillarData);
                }
                pillarsData.ReviewMode = true;
                break;
            }
            return pillarsData;
        }

        public void RefreshData()
        {
            SwitchViewButton.transform.localScaleTransition(Statics.Mode.SessionMode == SessionMode.Session ? Vector3.one : Vector3.zero, 0.5f);

            PillarsManager.SetFocus();
            AnimateToUnfocused();

            bool showOnlyNewlyAddedCards = ViewMode == prevViewMode;
            prevViewMode = ViewMode;
            var pillarsData = RegenerateData();
            PillarsManager.ShowData(pillarsData, showOnlyNewlyAddedCards);
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
            Statics.Screens.OnSwitchToScreen -= OnSwitchToScreen;
            Statics.Screens.OnSwitchToScreen += OnSwitchToScreen;

            RefreshData();

            PillarsTutorial.gameObject.SetActive(Statics.SessionFlow.IsInsideTutorial);

            return base.OnOpen();
        }

        protected override IEnumerator OnClose()
        {
            yield return CloseCardList();
            yield return base.OnClose();
        }

        #region Interaction

        private bool isFocused;
        public void HandleSelectPillar(PillarView pillarView, int iPillar)
        {
            if (Statics.SessionFlow.IsInsideTutorial) return;

            if (PillarsManager.CurrentFocusedPillar == pillarView)
                return;
            AnimateToFocused();

            PillarsManager.SetFocus(pillarView);

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
            uiCardListScreen.Open();


            if (UICardListScreen.FRONT_VIEW_ONLY)
            {
                // Directly open the front view
                if (uiCardListScreen.CardsList.HeldCards.Count > 0)
                {
                    uiCardListScreen.OpenFrontView(uiCardListScreen.CardsList.HeldCards[0], desiredFrontViewMode);
                }
            }
        }

        private void HandleSelectPlane()
        {
            if (Statics.SessionFlow.IsInsideTutorial) return;

            if (!isFocused)
                return;
            AnimateToUnfocused();
            PillarsManager.SetFocus();

            StartCoroutine(CloseCardList());
        }

        private IEnumerator CloseCardList()
        {
            var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
            if (uiCardListScreen.IsOpen)
                uiCardListScreen.Close();
            while (uiCardListScreen.IsOpen)
                yield return null;
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
                        Camera3D.transform.localRotationTransition(Quaternion.Euler(41.5f, 10f, 0f), 0.25f, LeanEase.Decelerate);
                    }
                    else
                    {
                        isFocused = false;
                        Camera3D.transform.localRotationTransition(Quaternion.Euler(41.5f, -90f, 0f), 0.25f, LeanEase.Decelerate);
                    }
                    break;
                case ScreenID.Activities:
                    isFocused = false;
                    Camera3D.transform.localRotationTransition(Quaternion.Euler(41.5f, 50f, 0f), 0.25f, LeanEase.Decelerate);
                    break;
            }
        }

        #region Animations

        private void AnimateToFocused()
        {
            if (isFocused)
                return;
            isFocused = true;
            Camera3D.transform.localPositionTransition(new Vector3(0, 10.5f, -13), 0.5f);
            Camera3D.transform.localRotationTransition(new Quaternion(0.276550651f, 0.0829450935f, -0.0267626494f, 0.957038999f), 0.5f);
        }

        private void AnimateToUnfocused()
        {
            isFocused = false;
            Camera3D.transform.localPositionTransition(new Vector3(0, 15.1000004f, -13.3999996f), 0.5f);
            Camera3D.transform.localRotationTransition(new Quaternion(0.354291022f, 0, 0, 0.935135245f), 0.5f);

            PillarsManager.RefreshPositionsAndRotations(animated: true);
        }

        #endregion

    }
}
