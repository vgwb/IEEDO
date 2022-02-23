using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Transition;
using UnityEngine;

namespace Ieedo
{
    public enum PillarsViewMode
    {
        Categories,
        Review
    }

    public class UIPillarsScreen : UIScreen
    {
        public UICardCollection SelectedCardList;

        public override bool AutoAnimate => false;

        public GameObject Scene3D;
        public PillarsManager PillarsManager;

        public override ScreenID ID => ScreenID.Pillars;

        public PillarsViewMode ViewMode = PillarsViewMode.Categories;

        public void SwitchViewMode(PillarsViewMode newMode)
        {
            ViewMode = newMode;
            StartCoroutine(OnOpen());
        }

        protected override IEnumerator OnOpen()
        {
            Statics.Screens.OnSwitchToScreen -= this.OnSwitchToScreen;
            Statics.Screens.OnSwitchToScreen += this.OnSwitchToScreen;

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
                            Cards = cards.ToList()
                        };

                        pillarsData.Pillars.Add(pillarData);
                    }
                    break;
                case PillarsViewMode.Review:
                    {
                        var completedCards = Statics.Data.Profile.Cards.Where(x => x.Status == CardValidationStatus.Completed);
                        var validatedCards = Statics.Data.Profile.Cards.Where(x => x.Status == CardValidationStatus.Validated);

                        var pillarData = new PillarData
                        {
                            Color = Color.red,
                            Height = 0.5f,
                            Cards = completedCards.ToList(),
                        };
                        pillarsData.Pillars.Add(pillarData);

                        pillarData = new PillarData
                        {
                            Color = Color.yellow,
                            Height = 0.5f,
                            Cards = validatedCards.ToList(),
                        };
                        pillarsData.Pillars.Add(pillarData);
                    }
                    break;
            }

            PillarsManager.ShowData(pillarsData);
            Scene3D.SetActive(true);

            foreach (var pillarView in PillarsManager.PillarViews)
            {
                pillarView.OnSelected = () => HandleSelectedPillar(pillarView);
            }

            return base.OnOpen();
        }

        private void HandleSelectedPillar(PillarView pillarView)
        {
            var data = pillarView.Data;

            var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
            uiCardListScreen.LoadCards(data.Cards, UICardListScreen.SortByExpirationDate, UICardListScreen.ListViewMode.Review);
            uiCardListScreen.KeepPillars = true;
            GoTo(ScreenID.CardList);
        }

        protected override IEnumerator OnClose()
        {
           // Scene3D.SetActive(false);
            return base.OnClose();
        }

        public Camera Camera3D;
        public void OnSwitchToScreen(ScreenID screenID)
        {
            switch (screenID)
            {
                case ScreenID.CardList:
                    var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
                    if (uiCardListScreen.KeepPillars)
                    {
                        Camera3D.transform.localRotationTransition(Quaternion.Euler(34f,10f,0f), 0.25f, LeanEase.Decelerate);
                    }
                    else
                    {
                        Camera3D.transform.localRotationTransition(Quaternion.Euler(34f,50f,0f), 0.25f, LeanEase.Decelerate);
                    }
                    break;
                case ScreenID.Pillars:
                    Camera3D.transform.localRotationTransition(Quaternion.Euler(34f,0f,0f), 0.25f, LeanEase.Decelerate);
                    break;
                case ScreenID.Activities:
                    Camera3D.transform.localRotationTransition(Quaternion.Euler(34f,-10f,0f), 0.25f, LeanEase.Decelerate);
                    break;
            }
        }
    }
}