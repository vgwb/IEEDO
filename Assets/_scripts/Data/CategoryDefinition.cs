using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu(fileName = "Category", menuName = "Ieedo/Category")]

    public class CategoryDefinition : ScriptableObject, IDefinition
    {
        public CategoryID ID;
        public LocString Title;
        public LocString Description;
        public PaletteColor PaletteColor;
        public Color LightColor => PaletteColor.LightColor;
        public Color BaseColor => PaletteColor.BaseColor;
        public Color DarkColor => PaletteColor.DarkColor;

        public SubCategoryDefinition[] SubCategories;

        public int Id => (int)ID;
    }
}
