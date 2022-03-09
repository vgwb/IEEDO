using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Transition;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    public enum PillarsViewMode
    {
        Categories,
        Review
    }

    public class UIPillarsScreen : UIScreen
    {
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
                            Color = Color.red,
                            Height = 0.5f,
                            Cards = completedCards.ToList(),
                            LocalizedKey = new LocalizedString("UI","pillar_completed"),
                            PrioritizeLabel = true,
                        };
                        pillarsData.Pillars.Add(pillarData);

                        pillarData = new PillarData
                        {
                            Color = Color.yellow,
                            Height = 0.5f,
                            Cards = validatedCards.ToList(),
                            LocalizedKey = new LocalizedString("UI","pillar_validated"),
                            PrioritizeLabel = true,
                        };
                        pillarsData.Pillars.Add(pillarData);
                    }
                    PillarsManager.SetFocus(false, PillarsManager.PillarViews[0]);
                    break;
            }

            Camera3D.transform.localPositionTransition(new Vector3(0,15.1000004f,-13.3999996f), 0.5f);
            Camera3D.transform.localRotationTransition(new Quaternion(0.29237175f,0,0,0.956304789f), 0.5f);


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

            PillarsManager.SetFocus(false, pillarView);
            pillarView.ShowLabel();

            Camera3D.transform.localPositionTransition(new Vector3(0, 10.5f, -13), 0.5f);
            Camera3D.transform.localRotationTransition(new Quaternion(0.255145639f,0.0823278204f,-0.0286051836f,0.962966561f), 0.5f);

            var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
            uiCardListScreen.LoadCards(data.Cards, UICardListScreen.SortByExpirationDate, UICardListScreen.ListViewMode.Review);
            uiCardListScreen.KeepPillars = true;
            GoTo(ScreenID.CardList);
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
                        Camera3D.transform.localRotationTransition(Quaternion.Euler(34f,-50f,0f), 0.25f, LeanEase.Decelerate);
                    }
                    break;
                case ScreenID.Pillars:
                    Camera3D.transform.localRotationTransition(Quaternion.Euler(34f,0f,0f), 0.25f, LeanEase.Decelerate);
                    break;
                case ScreenID.Activities:
                    Camera3D.transform.localRotationTransition(Quaternion.Euler(34f,10f,0f), 0.25f, LeanEase.Decelerate);
                    break;
            }
        }
    }
}
