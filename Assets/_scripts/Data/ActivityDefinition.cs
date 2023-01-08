using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu(fileName = "Activity", menuName = "Ieedo/Activity")]
    public class ActivityDefinition : ScriptableObject, IDefinition
    {
        public bool Enabled;
        public bool Available;
        public ActivityID ID;
        public LocString Title;
        public ActivityType Type;
        public ScoreType ScoreType;
        public ActivityCategory Category;
        public ActivitySkills Skills;
        public Sprite Image;
        public string SceneName;
        public int PointsToUnlock;
        public int PointsOnWin = 100;
        public int PointsOnLoss = 20;
        public int MaxLevel = 10;

        public int Id => (int)ID;
        public string LocName => $"{Type}_{ID}".ToLower();
    }
}
