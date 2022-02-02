using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ieedo.Test
{
    [System.Serializable]
    public class PillarData
    {
        public float Height;
        public Color Color = Color.white;
        public int NCards;
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
            if (!TEST) return;
            ShowData(TestData);

            transform.localEulerAngles += Vector3.up * Time.deltaTime * RotationSpeed;
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
                var rot = Quaternion.AngleAxis(360f / n * iPillar, Vector3.up);
                locPos = rot * delta;
                pillarView.transform.localPosition = locPos;
                var locEul = rot.eulerAngles;
                locEul.y += 180;
                pillarView.transform.localEulerAngles = locEul;

                pillarView.ShowData(pillarData);
                pillarView.Show();
            }
        }
    }
}