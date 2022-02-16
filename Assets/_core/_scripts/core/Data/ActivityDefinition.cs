using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu]
    public class ActivityDefinition : ScriptableObject, IDefinition
    {
        public ActivityID ID;
        public LocalizedString Title;
        public LocalizedString Description;
        public ActivityType Type;
        public string SceneName;
        public int ScoreToUnlock;

        public int Id => (int)ID;
    }
}