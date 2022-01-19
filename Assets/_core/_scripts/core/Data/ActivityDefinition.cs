using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu]
    public class ActivityDefinition : ScriptableObject
    {
        public int ID;
        public LocalizedString Title;
        public LocalizedString Description;
        public PaletteColor Color;
        public ActivityType Type;
    }
}