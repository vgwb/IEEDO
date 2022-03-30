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
            if (autoRotating && currentFocusedPillar == null) transform.localEulerAngles += Vector3.up * Time.deltaTime * RotationSpeed;

            if (!TEST) return;
            ShowData(TestData, true);
        }

        public List<PillarView> PillarViews = new List<PillarView>();
        private PillarsData currentData;

        public void ShowData(PillarsData data, bool added, int onlyPillarIndex = -1)
        {
            this.currentData = data;
            foreach (var pillarView in PillarViews)
                pillarView.Hide();

            for (var iPillar = 0; iPillar < data.Pillars.Count; iPillar++)
            {
                var pillarView = PillarViews[iPillar];

                var locPos = pillarView.transform.localPosition;
                var delta = Vector3.forward * Radius;
                var deltaAngle = 360f / 6;
                if (data.ReviewMode) deltaAngle = 360f / 2;

                var startRot = 0;
                if (data.ReviewMode) startRot = 90;
                var rot = Quaternion.AngleAxis(startRot + deltaAngle * iPillar, Vector3.up);

                locPos = rot * delta;
                pillarView.transform.localPosition = locPos;
                var locEul = rot.eulerAngles;
                locEul.y += 180;
                pillarView.transform.localEulerAngles = locEul;
                if (data.ReviewMode)
                {
                    var finalRot = locEul;
                    finalRot.y += iPillar == 0 ? 90 : -90;
                    pillarView.transform.localRotationTransition(Quaternion.Euler(finalRot), 0.25f);
                }

                if (onlyPillarIndex < 0 || onlyPillarIndex == iPillar)
                {
                    RefreshPillarView(iPillar, added);
                }
                pillarView.Show();
            }

            autoRotating = !data.ReviewMode;
            if (!autoRotating) transform.localRotationTransition(Quaternion.identity, 0.5f);
        }

        public void RefreshPillarView(int iPillar, bool added)
        {
            var pillarData = currentData.Pillars[iPillar];
            var pillarView = PillarViews[iPillar];
            pillarView.ShowData(pillarData, added, currentData.ReviewMode);
        }

        private bool autoRotating;
        private PillarView currentFocusedPillar;
        public PillarView CurrentFocusedPillar => currentFocusedPillar;
        public void SetFocus(PillarView focusOnPillar = null)
        {
            if (focusOnPillar != null && focusOnPillar == currentFocusedPillar) return;

            if (currentFocusedPillar != null)
            {
                currentFocusedPillar.ShowLabel(false);
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
                if (!autoRotating) transform.localRotationTransition(Quaternion.identity, 0.5f);
            }
        }

    }
}
