using System.Collections;
using System.Linq;
using UnityEngine;

namespace Ieedo
{
    public class AssessmentFlowManager : MonoBehaviour
    {
        public UIQuestionPopup QuestionPopup;

        public void StartAssessment()
        {
            StartCoroutine(AssessmentCO());
        }

        public IEnumerator AssessmentCO()
        {
            var categories = Statics.Data.GetAll<CategoryDefinition>();
            foreach (var category in categories)
            {
                var questions = Statics.Data.GetAll<AssessmentQuestionDefinition>();
                questions = questions.Where(x => x.Category == category.ID).ToList();
                foreach (var question in questions)
                {
                    QuestionPopup.ShowQuestion(question);
                    while (QuestionPopup.isActiveAndEnabled) yield return null;
                    var selectedAnswer = question.Answers[QuestionPopup.LatestSelectedOption];
                }
            }
        }
    }
}