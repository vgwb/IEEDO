using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;

namespace Ieedo
{
    public class SessionFlowManager : MonoBehaviour
    {
        public Image BlockerBG;

        public bool IsInSessionFlow = false;

        public IEnumerator SessionFlowCO()
        {
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;

            IsInSessionFlow = true;
            Statics.Screens.GoTo(ScreenID.Pillars);

            // Assessment flow
            var answer = new Ref<int>();
            yield return Statics.Screens.ShowQuestionFlow("UI/session_question_assessment_title", "UI/session_question_assessment_content", new[] { "UI/yes", "UI/no" }, answer);
            if (answer.Value == 0)
            {
                yield return AssessmentFlowCO();
            }

            // Review flow
            uiPillarsScreen.SwitchViewMode(PillarsViewMode.Review);
            yield return new WaitForSeconds(0.5f);
            bool hasCardsToValidate = Statics.Data.Profile.Cards.HasCardsWithStatus(CardValidationStatus.Completed);
            if (hasCardsToValidate)
            {
                yield return Statics.Screens.ShowDialog("UI/session_hint_review","UI/ok");

                uiPillarsScreen.HandleSelectPillar(uiPillarsScreen.PillarsManager.PillarViews[UICardListScreen.COMPLETED_CARDS_PILLAR_INDEX], UICardListScreen.COMPLETED_CARDS_PILLAR_INDEX);
                yield return new WaitForSeconds(0.5f);

                do
                {
                    uiCardListScreen.OpenFrontView(uiCardListScreen.CardsList.HeldCards[0], UICardListScreen.FrontViewMode.Completed);
                    while (uiCardListScreen.CurrentFrontViewMode != UICardListScreen.FrontViewMode.None)
                    {
                        yield return null;
                    }
                    yield return null;
                }
                while (Statics.Data.Profile.Cards.HasCardsWithStatus(CardValidationStatus.Completed));
            }
            yield return Statics.Screens.ShowDialog("UI/session_hint_review_empty","UI/ok");

            // Creation flow
            Statics.Screens.GoToTodoList();
            yield return new WaitForSeconds(0.5f);
            yield return Statics.Screens.ShowDialog("UI/session_hint_create_card","UI/ok");

            IsInSessionFlow = false;
        }

        public void StartAssessment()
        {
            StartCoroutine(AssessmentFlowCO());
        }

        public IEnumerator AssessmentFlowCO()
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
