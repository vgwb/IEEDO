using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu]
    public class ActivityDefinition : ScriptableObject, IDefinition
    {
        public ActivityID ID;
        public LocalizedString Title;
        public LocalizedString Description;
        public PaletteColor Color;
        public ActivityType Type;
        public string SceneName;
        public int Id => (int)ID;
    }
}