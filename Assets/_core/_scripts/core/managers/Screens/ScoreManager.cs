using UnityEngine;

namespace Ieedo
{
    public class ScoreManager : MonoBehaviour
    {
        public UIText CurrentScoreText;
        public LocalizedString ScoreLoc;

        void Awake()
        {
            //ScoreLoc.Key.Arguments.Add(Statics.Data.Profile.CurrentScore);
            ScoreLoc.Key.StringChanged += v => CurrentScoreText.text = v;// $"Score: {Statics.Data.Profile.CurrentScore.ToString(),5}";
        }

        public void AddScore(int value)
        {
            Statics.Data.Profile.CurrentScore += value;
            CurrentScoreText.text = $"Score: {Statics.Data.Profile.CurrentScore.ToString(),5}";
            Statics.Data.SaveProfile();
        }
    }
}
