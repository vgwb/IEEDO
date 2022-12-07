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
        public Color Color => PaletteColor.Color;

        public SubCategoryDefinition[] SubCategories;

        public int Id => (int)ID;
    }

}