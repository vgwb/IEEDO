using System;
using UnityEngine;

namespace Ieedo
{
    [Serializable]
    public class AssessmentAnswer
    {
        public LocalizedString Answer;
        [Range(0,1)]
        public float Value = 1f;

        public AssessmentAnswer()
        {
        }

        public AssessmentAnswer(LocalizedString answer, float value)
        {
            Answer = answer;
            Value = value;
        }
    }

    [CreateAssetMenu]
    public class AssessmentQuestionDefinition : ScriptableObject
    {
        public int ID;
        public LocalizedString Question;
        public CategoryID Category;
        [Range(0,2)]
        public float Weight = 1f;

        public AssessmentAnswer[] Answers = {
            new(new("Yes"), 1f),
            new(new("No"), 1f),
        };
    }
}