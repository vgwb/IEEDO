using System;
using System.Collections.Generic;
using System.Numerics;
using Lean.Transition;
using UnityEngine;
using UnityEngine.Localization;
using Vector3 = UnityEngine.Vector3;

namespace Ieedo
{
    public class PointsManager : MonoBehaviour
    {
        public UIText CurrentPointsText;

        public void RefreshPointsText()
        {
            if (CurrentPointsText != null)
            {
                CurrentPointsText.transform.localScaleTransition(Vector3.one * 1.5f, 0.25f).JoinDelayTransition(0.25f).localScaleTransition(Vector3.one, 0.25f);
                CurrentPointsText.SetTextRaw($"{Statics.Data.Profile.CurrentPoints.ToString()}");
            }
        }

        public void AddPoints(int value)
        {
            SoundManager.I.PlaySfx(SfxEnum.score);
            Statics.Data.Profile.CurrentPoints += value;
            RefreshPointsText();
            Statics.Data.SaveProfile();
        }

    }
}
