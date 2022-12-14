using System.Collections.Generic;
using System.Numerics;
using Lean.Transition;
using UnityEngine;
using UnityEngine.Localization;
using Vector3 = UnityEngine.Vector3;

namespace Ieedo
{
    public class ScoreManager : MonoBehaviour
    {
        public UIText CurrentScoreText;


        public void AddScore(int value)
        {
            SoundManager.I.PlaySfx(SfxEnum.score);
            Statics.Data.Profile.CurrentScore += value;
            CurrentScoreText.transform.localScaleTransition(Vector3.one * 1.5f, 0.25f).JoinDelayTransition(0.25f).localScaleTransition(Vector3.one, 0.25f);
            CurrentScoreText.Text = $"{Statics.Data.Profile.CurrentScore.ToString()}";
            Statics.Data.SaveProfile();
        }

    }
}
