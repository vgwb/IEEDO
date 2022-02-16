using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ieedo
{
    public class AssessmentFlowManager : MonoBehaviour
    {
        public UIQuestionPopup QuestionPopup; // TODO: get from screens instead
        public UIAssessmentRecapPopup AssessmentRecapPopup; // TODO: get from screens instead

        public void StartAssessment()
        {
            StartCoroutine(AssessmentCO());
        }

        public IEnumerator AssessmentCO()
        {
            var introScreen = Statics.Screens.Get(ScreenID.AssessmentIntro) as UIAssessmentIntroScreen;
            yield return introScreen.ShowIntro();
            while (introScreen.IsOpen) yield return null;

            var overallValue = 0f;
            var assessmentPercentages = new Dictionary<int, float>();
            var categories = Statics.Data.GetAll<CategoryDefinition>();
            foreach (var category in categories)
            {
                var categoryIntroScreen = Statics.Screens.Get(ScreenID.AssessmentCategoryIntro) as UIAssessmentCategoryIntroScreen;
                yield return categoryIntroScreen.ShowCategory(category);
                while (categoryIntroScreen.isActiveAndEnabled) yield return null;

                int nQuestions = 0;
                var questions = Statics.Data.GetAll<AssessmentQuestionDefinition>();
                questions = questions.Where(x => x.Category == category.ID).ToList();
                float totValue = 0f;
                foreach (var question in questions)
                {
                    QuestionPopup.ShowQuestion(question);
                    while (QuestionPopup.IsOpen) yield return null;
                    var selectedAnswer = question.Answers[QuestionPopup.LatestSelectedOption];
                    totValue += selectedAnswer.Value;
                    nQuestions++;
                }
                if (nQuestions == 0) nQuestions = 1; // To avoid NaN
                assessmentPercentages[(int)category.ID] = totValue / nQuestions;
                overallValue += assessmentPercentages[(int)category.ID];
            }

            overallValue /= categories.Count;
            AssessmentRecapPopup.ShowResults(assessmentPercentages, overallValue);
            while (AssessmentRecapPopup.IsOpen) yield return null;

            var profileData = Statics.Data.Profile;
            foreach (var categoryData in profileData.Categories)
            {
                categoryData.AssessmentValue = assessmentPercentages[(int)categoryData.ID];
            }
            Statics.Data.SaveProfile();

            // Refresh pillars
            Statics.Screens.GoTo(ScreenID.Pillars);
        }
    }
}