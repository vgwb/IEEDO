using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu]
    public class AssessmentQuestionDefinition : ScriptableObject
    {
        public int ID;
        public LocalizedString Question;
        public LocalizedString Category;
        [Range(0,2)]
        public float Weight = 1f;
    }
}