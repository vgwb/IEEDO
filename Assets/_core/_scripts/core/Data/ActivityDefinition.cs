using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu(fileName = "Activity", menuName = "Ieedo/Activity")]
    public class ActivityDefinition : ScriptableObject, IDefinition
    {
        public ActivityID ID;
        public LocString Title;
        public ActivityType Type;
        public string SceneName;
        public int ScoreToUnlock;
        public int MaxLevel = 10;

        public int Id => (int)ID;
    }
}
