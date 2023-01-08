using System;

namespace Ieedo
{
    [Serializable]
    public class CategoryData
    {
        public CategoryID ID;
        public float AssessmentValue;

        public override string ToString()
        {
            return $"{ID}: {AssessmentValue}";
        }
    }
}
