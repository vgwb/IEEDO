using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ieedo
{
    public class AssessmentFlowManager : MonoBehaviour
    {
        public UIQuestionPopup QuestionPopup;
        public UIAssessmentRecapPopup AssessmentRecapPopup;

        public void StartAssessment()
        {
            StartCoroutine(AssessmentCO());
        }

        public IEnumerator AssessmentCO()
        {
            var overallValue = 0f;
            var assessmentPercentages = new Dictionary<int, float>();
            var categories = Statics.Data.GetAll<CategoryDefinition>();
            foreach (var category in categories)
            {
                int nQuestions = 0;
                var questions = Statics.Data.GetAll<AssessmentQuestionDefinition>();
                questions = questions.Where(x => x.Category == category.ID).ToList();
                float totValue = 0f;
                foreach (var question in questions)
                {
                    QuestionPopup.ShowQuestion(question);
                    while (QuestionPopup.isActiveAndEnabled) yield return null;
                    var selectedAnswer = question.Answers[QuestionPopup.LatestSelectedOption];
                    totValue += selectedAnswer.Value;
                    nQuestions++;
                }
                if (nQuestions == 0) nQuestions = 1; // To avoid NaN
                assessmentPercentages[(int)category.ID] = totValue / nQuestions;
                overallValue += totValue;
            }

            overallValue /= categories.Count;
            AssessmentRecapPopup.ShowResults(assessmentPercentages, overallValue);
            while (AssessmentRecapPopup.isActiveAndEnabled) yield return null;

            var profileData = Statics.Data.Profile;
            foreach (var categoryData in profileData.Categories)
            {
                categoryData.AssessmentValue = assessmentPercentages[(int)categoryData.ID];
            }
            Statics.Data.SaveProfile();
        }
    }
}