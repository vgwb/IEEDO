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

        // @note: used by Localization
        public string CurrentScore => $"{Statics.Data.Profile.CurrentScore.ToString(),5}";

        void Start()
        {
            if (CurrentScoreText != null)
            {
                CurrentScoreText.Key = new LocalizedString("UI", "top_score");
                CurrentScoreText.Key.Arguments = new List<object> { this };
                CurrentScoreText.Key.StringChanged += v =>
                {
                    CurrentScoreText.text = v;
                };
            }
        }

        public void AddScore(int value)
        {
            Statics.Data.Profile.CurrentScore += value;
            CurrentScoreText.transform.localScaleTransition(Vector3.one * 1.5f, 0.25f).JoinDelayTransition(0.25f).localScaleTransition(Vector3.one, 0.25f);
            CurrentScoreText.Key.RefreshString();
            Statics.Data.SaveProfile();
        }

        public void RefreshString()
        {
            CurrentScoreText?.Key.RefreshString();
        }
    }
}
