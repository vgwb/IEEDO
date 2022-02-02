using System;

namespace Ieedo
{
    [Serializable]
    public class CategoryData
    {
        public CategoryID ID;
        public int AssessmentValue;

        public CategoryDefinition Definition => Statics.Data.Get<CategoryDefinition>((int)ID);

        public override string ToString()
        {
            return $"{ID}: {AssessmentValue}";
        }
    }
}