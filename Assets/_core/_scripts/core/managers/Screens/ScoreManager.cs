using UnityEngine;

namespace Ieedo
{
    public class ScoreManager : MonoBehaviour
    {
        public UIText CurrentScoreText;

        public void AddScore(int value)
        {
            Statics.Data.Profile.CurrentScore += value;
            CurrentScoreText.text = $"Score: {Statics.Data.Profile.CurrentScore.ToString(),5}";
            Statics.Data.SaveProfile();
        }
    }
}