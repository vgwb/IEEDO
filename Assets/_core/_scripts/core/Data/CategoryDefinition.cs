using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu(fileName = "Category", menuName = "Ieedo/Category")]

    public class CategoryDefinition : ScriptableObject, IDefinition
    {
        public CategoryID ID;
        public LocalizedString Title;
        public LocalizedString Description;
        public PaletteColor PaletteColor;
        public Color Color => PaletteColor.Color;

        public SubCategoryDefinition[] SubCategories;

        public int Id => (int)ID;
    }

}