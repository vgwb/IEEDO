using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ieedo
{
    public class UIPillarsScreen : UIScreen
    {
        public override bool AutoAnimate => false;

        public GameObject Scene3D;
        public PillarsManager PillarsManager;

        public override ScreenID ID => ScreenID.Pillars;

        protected override IEnumerator OnOpen()
        {
            var profileData = Statics.Data.Profile;

            var pillarsData = new PillarsData
            {
                Pillars = new List<PillarData>()
            };
            foreach (var category in profileData.Categories)
            {
                var nCards = Statics.Data.Profile.Cards.Count(x => x.Status != CardValidationStatus.Todo && x.Definition.Category == category.ID);

                var pillarData = new PillarData
                {
                    Color = Statics.Data.Definition(category.ID).Color,
                    Height = category.AssessmentValue,
                    NCards = nCards,
                };

                pillarsData.Pillars.Add(pillarData);
            }
            PillarsManager.ShowData(pillarsData);
            Scene3D.SetActive(true);

            return base.OnOpen();
        }

        protected override IEnumerator OnClose()
        {
           // Scene3D.SetActive(false);
            return base.OnClose();
        }
    }
}