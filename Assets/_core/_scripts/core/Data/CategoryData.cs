using System;

namespace Ieedo
{
    [Serializable]
    public class CategoryData
    {
        public CategoryID ID;
        public int Value;

        public override string ToString()
        {
            return $"{ID}: {Value}";
        }
    }
}