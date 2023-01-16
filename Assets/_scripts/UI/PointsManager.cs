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
        public UIPoints PointsUI;

        // public void RefreshPointsText()
        // {
        //     if (PointsUI != null)
        //     {
        //         CurrentPointsText.transform.localScaleTransition(Vector3.one * 1.5f, 0.25f).JoinDelayTransition(0.25f).localScaleTransition(Vector3.one, 0.25f);
        //         CurrentPointsText.SetTextRaw($"{Statics.Data.Profile.CurrentPoints.ToString()}");
        //     }
        // }

        public void Init()
        {
            PointsUI?.UpdatePoints(Statics.Data.Profile.CurrentPoints, 0);
        }

        public void ShowPoints(bool visible)
        {
            PointsUI?.gameObject.SetActive(visible);
        }

        public void AddPoints(int value)
        {
            Statics.Data.Profile.CurrentPoints += value;
            PointsUI?.UpdatePoints(Statics.Data.Profile.CurrentPoints, value);
            Statics.Data.SaveProfile();
        }

    }
}
