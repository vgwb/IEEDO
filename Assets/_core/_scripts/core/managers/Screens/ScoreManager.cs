using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{
    public class ScoreManager : MonoBehaviour
    {
        public UIText CurrentScoreText;

        // @note: used by Localization
        public string CurrentScore => $"{Statics.Data.Profile.CurrentScore.ToString(),5}";

        void Awake()
        {
            if (CurrentScoreText != null)
            {
                CurrentScoreText.Key.Arguments = new List<object>{this};
                CurrentScoreText.Key.StringChanged += v => CurrentScoreText.text = v;
            }
        }

        public void AddScore(int value)
        {
            Statics.Data.Profile.CurrentScore += value;
            CurrentScoreText.Key.RefreshString();
            Statics.Data.SaveProfile();
        }
    }
}
