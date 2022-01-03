using UnityEngine;

namespace Ieedo
{
    public enum CategoryID
    {
        None,
        A,
        B,
        C
    }

    [CreateAssetMenu]
    public class CategoryDefinition : ScriptableObject
    {
        public CategoryID ID;
        public LocalizedString Title;
        public LocalizedString Description;
        public PaletteColor Color;
    }
}