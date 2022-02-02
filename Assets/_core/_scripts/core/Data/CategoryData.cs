using System;

namespace Ieedo
{
    [Serializable]
    public class CategoryData
    {
        public CategoryID ID;
        public int AssessmentValue;

        public override string ToString()
        {
            return $"{ID}: {AssessmentValue}";
        }
    }
}