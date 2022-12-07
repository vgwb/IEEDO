using System;

namespace Ieedo
{
    [Serializable]
    public class SubCategoryDefinition
    {
        public SubCategoryID ID;
        public LocString Title;
        public string Icon;

        public int Id => (int)ID;
    }
}