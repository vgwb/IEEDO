using UnityEngine;

namespace Ieedo
{
    public class ModeManager : MonoBehaviour
    {
        public UIText ModeText;

        public void AddScore(int value)
        {
            Statics.Data.Profile.CurrentScore += value;
            CurrentScoreText.text = $"Score: {Statics.Data.Profile.CurrentScore.ToString(),5}";
        }
    }
}