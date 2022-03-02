using System;
using UnityEngine;

namespace Ieedo
{
    [Serializable]
    public class AssessmentAnswer
    {
        public LocString Answer;
        [Range(0, 1)]
        public float Value = 1f;

        public AssessmentAnswer()
        {
        }

        public AssessmentAnswer(LocString answer, float value)
        {
            Answer = answer;
            Value = value;
        }
    }

    [CreateAssetMenu(fileName = "AssessmentQuestion", menuName = "Ieedo/Assessment Question")]
    public class AssessmentQuestionDefinition : ScriptableObject
    {
        public int ID;
        public LocString Question;
        public CategoryID Category;
        [Range(0, 2)]
        public float Weight = 1f;

        public AssessmentAnswer[] Answers = {
            new(new("UI","yes"), 1f),
            new(new("UI","no"), 1f),
        };
    }
}
