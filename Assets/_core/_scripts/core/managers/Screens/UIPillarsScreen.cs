using System.Collections;
using System.Collections.Generic;
using Ieedo.Test;
using UnityEngine;

namespace Ieedo
{
    public class UIPillarsScreen : UIScreen
    {
        public GameObject Scene3D;
        public PillarsManager PillarsManager;

        public override ScreenID ID => ScreenID.Pillars;

        void Start()
        {
        }

        protected override IEnumerator OnOpen()
        {
            var profileData = Statics.Data.Profile;

            var pillarsData = new PillarsData
            {
                Pillars = new List<PillarData>()
            };
            foreach (var category in profileData.Categories)
            {
                var pillarData = new PillarData
                {
                    Color = Statics.Data.Definition(category.ID).PaletteColor.Color,
                    Height = category.AssessmentValue * 0.2f, // multiplier
                    NCards = 0,
                };

                pillarsData.Pillars.Add(pillarData);
            }
            PillarsManager.ShowData(pillarsData);
            Scene3D.SetActive(true);
            return base.OnOpen();
        }

        protected override IEnumerator OnClose()
        {
            Scene3D.SetActive(false);
            return base.OnClose();
        }
    }
}