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
        public float Height;
        public Color Color = Color.white;
        public int NCards => Cards.Count;
        public List<CardData> Cards;
        public LocalizedString LocalizedKey;
        public bool PrioritizeLabel;
    }

    [System.Serializable]
    public class PillarsData
    {
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
            if (autoRotating) transform.localEulerAngles += Vector3.up * Time.deltaTime * RotationSpeed;

            if (!TEST) return;
            ShowData(TestData);
        }

        public List<PillarView> PillarViews = new List<PillarView>();

        public void ShowData(PillarsData data)
        {
            foreach (var pillarView in PillarViews)
                pillarView.Hide();

            var n = data.Pillars.Count;
            for (var iPillar = 0; iPillar < data.Pillars.Count; iPillar++)
            {
                var pillarData = data.Pillars[iPillar];
                var pillarView = PillarViews[iPillar];

                var locPos = pillarView.transform.localPosition;
                var delta = Vector3.forward * Radius;
                var deltaAngle = 360f / 6;
                var rot = Quaternion.AngleAxis(deltaAngle * iPillar, Vector3.up);
                locPos = rot * delta;
                pillarView.transform.localPosition = locPos;
                var locEul = rot.eulerAngles;
                locEul.y += 180;
                pillarView.transform.localEulerAngles = locEul;

                pillarView.ShowData(pillarData);
                pillarView.Show();
            }
        }

        private bool autoRotating;
        public void SetFocus(bool _autoRotating, PillarView focusOnPillar = null)
        {
            var targetRot = Quaternion.identity;
            if (focusOnPillar != null)
            {
                var dir = focusOnPillar.transform.localPosition;
                dir = Quaternion.Euler(0, 180, 0) * dir;
                targetRot = Quaternion.LookRotation(dir);
                var targetEuls = targetRot.eulerAngles;
                targetEuls.y *= -1;
                targetRot = Quaternion.Euler(targetEuls);
            }
            autoRotating = _autoRotating;
            if (!_autoRotating) transform.localRotationTransition(targetRot, 0.5f);
        }

    }
}
