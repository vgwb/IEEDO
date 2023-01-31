using System.Collections;
using System.Collections.Generic;
using Lean.Transition;
using UnityEngine;
using UnityEngine.Localization;

namespace Ieedo
{
    [System.Serializable]
    public class PillarData
    {
        public string IconString;
        public float Height;
        public Color Color = Color.white;
        public int NCards => Cards.Count;
        public List<CardData> Cards;
        public LocalizedString LocalizedKey;
    }

    [System.Serializable]
    public class PillarsData
    {
        public bool ReviewMode;
        public List<PillarData> Pillars = new List<PillarData>();
    }

    public class PillarsManager : MonoBehaviour
    {
        public float Radius = 1;
        public float RotationSpeed = 30f;

        public bool TEST;
        public PillarsData TestData = new PillarsData();

        public void Update()
        {
            if (autoRotating && currentFocusedPillar == null)
            {
                transform.localEulerAngles += Vector3.up * Time.deltaTime * RotationSpeed;

                // Detect the front pillar while rotating
                int nPillars = 6;
                var iFrontPillar = (nPillars - 1) - (int)((30f + transform.localEulerAngles.y) / (360 / nPillars));
                iFrontPillar -= 2;
                if (iFrontPillar < 0)
                    iFrontPillar += nPillars;
                for (int i = 0; i < PillarViews.Count; i++)
                {
                    PillarViews[i].ShowLabel(iFrontPillar == i);
                }

            }

            if (!TEST)
                return;
            ShowData(TestData, true);
        }

        public void RemoveData()
        {
            AssignData(null);
            foreach (var pillarView in PillarViews)
                pillarView.Hide();
        }

        public List<PillarView> PillarViews = new List<PillarView>();
        private PillarsData currentData;

        public void AssignData(PillarsData data)
        {
            this.currentData = data;
        }

        public void RefreshPositionsAndRotations(bool animated = false)
        {
            animated = false; // TODO: fix this
            if (currentData == null)
                return;
            var data = currentData;
            for (var iPillar = 0; iPillar < data.Pillars.Count; iPillar++)
            {
                var pillarView = PillarViews[iPillar];

                var locPos = pillarView.transform.localPosition;
                var delta = Vector3.forward * Radius;
                var deltaAngle = 360f / 6;
                if (data.ReviewMode)
                    deltaAngle = 360f / 2;

                var startRot = 0;
                if (data.ReviewMode)
                    startRot = 90;
                var rot = Quaternion.AngleAxis(startRot + deltaAngle * iPillar, Vector3.up);

                locPos = rot * delta;
                pillarView.transform.localPosition = locPos;
                var locEul = rot.eulerAngles;
                locEul.y += 180;
                if (data.ReviewMode)
                {
                    locEul.y += iPillar == 0 ? 90 : -90;
                }
                if (animated)
                    pillarView.transform.localEulerAnglesTransform(locEul, 0.5f);
                else
                    pillarView.transform.localEulerAngles = locEul;
            }

        }

        public void ShowData(PillarsData data, bool showOnlyNewlyAddedCards)
        {
            AssignData(data);
            foreach (var pillarView in PillarViews)
                pillarView.Hide();

            RefreshPositionsAndRotations();

            for (var iPillar = 0; iPillar < data.Pillars.Count; iPillar++)
            {
                var pillarView = PillarViews[iPillar];
                RefreshPillarView(iPillar, showOnlyNewlyAddedCards);
                pillarView.Show();
            }

            autoRotating = !data.ReviewMode;
            if (!autoRotating)
                transform.localRotation = Quaternion.identity;
        }

        public void RefreshPillarView(int iPillar, bool showOnlyNewlyAddedCards)
        {
            var pillarData = currentData.Pillars[iPillar];
            var pillarView = PillarViews[iPillar];
            pillarView.ShowData(pillarData, showOnlyNewlyAddedCards);
        }

        private bool autoRotating;
        private PillarView currentFocusedPillar;
        public PillarView CurrentFocusedPillar => currentFocusedPillar;
        public void SetFocus(PillarView focusOnPillar = null)
        {
            if (focusOnPillar != null && focusOnPillar == currentFocusedPillar)
                return;
            //SoundManager.I.PlaySfx(SfxEnum.panel_open);
            foreach (PillarView view in PillarViews)
            {
                if (view != focusOnPillar)
                {
                    view.ShowLabel(false);
                }
            }

            if (currentFocusedPillar != null)
            {
                currentFocusedPillar.CardsIn();
                currentFocusedPillar = null;
            }

            if (focusOnPillar != null)
            {
                var dir = focusOnPillar.transform.localPosition;
                dir = Quaternion.Euler(0, 180, 0) * dir;
                var targetRot = Quaternion.LookRotation(dir);
                var targetEuls = targetRot.eulerAngles;
                targetEuls.y *= -1;
                targetRot = Quaternion.Euler(targetEuls);
                focusOnPillar.ShowLabel(true);
                focusOnPillar.CardsOut();
                currentFocusedPillar = focusOnPillar;
                transform.localRotationTransition(targetRot, 0.5f);

                focusOnPillar.transform.localRotationTransition(Quaternion.LookRotation(dir), 0.25f);
            }
            else
            {
                if (!autoRotating)
                    transform.localRotationTransition(Quaternion.identity, 0.5f);
            }
        }

    }
}
