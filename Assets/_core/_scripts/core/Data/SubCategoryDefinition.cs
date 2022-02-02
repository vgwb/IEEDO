using System;

namespace Ieedo
{
    [Serializable]
    public class SubCategoryDefinition
    {
        public SubCategoryID ID;
        public LocalizedString Title;
        public string Icon;

        public int Id => (int)ID;
    }
}