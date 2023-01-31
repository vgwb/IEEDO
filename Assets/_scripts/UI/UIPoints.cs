using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Ieedo
{
    public class UIPoints : MonoBehaviour
    {
        public TextMeshProUGUI PointsText;
        public TextMeshProUGUI IconText;
        public GameObject IconGO;

        // private Sequence PunchAnimation;

        void Start()
        {
            IconText.text = Statics.App.ApplicationConfig.PointsSymbol;
            //     PunchAnimation = DOTween.Sequence()
            //    .Insert(0, IconGO.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 1, 3, 0f))
            //    .SetAutoKill(false).Pause();
        }

        public void UpdatePoints(int totalpoints, int delta)
        {
            PointsText.text = totalpoints.ToString();
            // PunchAnimation.Rewind();
            // PunchAnimation.Play();
            SoundManager.I.PlaySfx(AudioEnum.points);
        }

    }
}
