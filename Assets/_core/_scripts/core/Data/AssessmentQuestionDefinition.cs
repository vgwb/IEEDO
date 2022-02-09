using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu]
    public class AssessmentQuestionDefinition : ScriptableObject
    {
        public int ID;
        public LocalizedString Question;
        public CategoryID Category;
        [Range(0,2)]
        public float Weight = 1f;

        public LocalizedString[] Answers = new LocalizedString[]
        {
            new LocalizedString() { DefaultText = "Yes" },
            new LocalizedString() { DefaultText = "No" }
        };
    }
}