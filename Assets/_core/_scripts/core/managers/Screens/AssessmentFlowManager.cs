using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;

namespace Ieedo
{
    public class AssessmentFlowManager : MonoBehaviour
    {
        public Image BlockerBG;

        public void StartAssessment()
        {
            StartCoroutine(AssessmentCO());
        }

        public IEnumerator AssessmentCO()
        {
            var questionScreen = Statics.Screens.Get(ScreenID.Question) as UIQuestionPopup;
            var assessmentRecapScreen = Statics.Screens.Get(ScreenID.AssessmentRecap) as UIAssessmentRecapPopup;
            var introScreen = Statics.Screens.Get(ScreenID.AssessmentIntro) as UIAssessmentIntroScreen;

            if (BlockerBG != null)
            {
                var col = BlockerBG.color; col.a = 1f;
                BlockerBG.colorTransition(col, 0.25f);
            }

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
                    yield return questionScreen.ShowQuestion(question);
                    while (questionScreen.IsOpen) yield return null;
                    var selectedAnswer = question.Answers[questionScreen.LatestSelectedOption];
                    totValue += selectedAnswer.Value;
                    nQuestions++;
                }
                if (nQuestions == 0) nQuestions = 1; // To avoid NaN
                assessmentPercentages[(int)category.ID] = totValue / nQuestions;
                overallValue += assessmentPercentages[(int)category.ID];
            }

            overallValue /= categories.Count;
            assessmentRecapScreen.ShowResults(assessmentPercentages, overallValue);
            while (assessmentRecapScreen.IsOpen) yield return null;

            var profileData = Statics.Data.Profile;
            foreach (var categoryData in profileData.Categories)
            {
                categoryData.AssessmentValue = assessmentPercentages[(int)categoryData.ID];
            }
            Statics.Data.SaveProfile();

            if (BlockerBG != null)
            {
                var col = BlockerBG.color; col.a = 0f;
                BlockerBG.colorTransition(col, 0.25f);
            }

            // Refresh pillars
            Statics.Screens.GoTo(ScreenID.Pillars);
        }
    }
}
