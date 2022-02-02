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
    public class CategoryDefinition : ScriptableObject, IDefinition
    {
        public CategoryID ID;
        public LocalizedString Title;
        public LocalizedString Description;
        public PaletteColor PaletteColor;
        public Color Color => PaletteColor.Color;

        public int Id => (int)ID;
        public string Icon;
    }
}