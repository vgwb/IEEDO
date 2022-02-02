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
            var data = new PillarsData()
            {
                Pillars = new List<PillarData>()
                {
                    new PillarData()
                    {
                        Color = Color.yellow,
                        Height = 2f,
                        NCards = 4,
                    }
                }
            };
            PillarsManager.ShowData(data);
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