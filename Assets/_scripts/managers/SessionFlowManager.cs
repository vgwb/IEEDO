using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Ieedo
{
    public class SessionFlowManager : MonoBehaviour
    {
        public Image BlockerBG;
        public bool IsInsideAssessment;
        public bool IsInsideTutorial;

        public Coroutine assessmentCo;
        public Coroutine sessionFlowCo;

        public void StartSessionMode()
        {
            if (Statics.App.ApplicationConfig.DebugMode)
            {

            }
            else
            {
                StartCoroutine(Statics.SessionFlow.SessionFlowCO());
            }
        }
        public IEnumerator SessionFlowCO()
        {
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;

            // TODO: Close everything else first?
            Statics.Screens.GoTo(ScreenID.Pillars);

            // Assessment flow
            var answer = new Ref<int>();

            if (!Statics.SessionFlow.IsInsideTutorial)
            {
                yield return Statics.Screens.ShowQuestionFlow("UI/session_question_assessment_title", "UI/session_question_assessment_content", new[] { "UI/yes", "UI/no" }, answer);
            }

            if (answer.Value == 0)
            {
                IsInsideAssessment = true;
                assessmentCo = StartCoroutine(AssessmentFlowCO()); // @note: needs to be a StartCoroutine so we can stop it
                while (IsInsideAssessment)
                    yield return null;
                assessmentCo = null;
            }
            else
            {
                // Review flow
                uiPillarsScreen.SwitchViewMode(PillarsViewMode.Review);
                yield return new WaitForSeconds(0.5f);
                bool hasCardsToValidate = Statics.Data.Profile.Cards.HasCardsWithStatus(CardStatus.Completed);
                if (hasCardsToValidate)
                {
                    yield return Statics.Screens.ShowDialog("UI/session_hint_review", "UI/ok");
                    uiPillarsScreen.HandleSelectPillar(uiPillarsScreen.PillarsManager.PillarViews[UICardListScreen.COMPLETED_CARDS_PILLAR_INDEX], UICardListScreen.COMPLETED_CARDS_PILLAR_INDEX);
                }
                else
                {
                    yield return Statics.Screens.ShowDialog("UI/session_hint_review_empty", "UI/ok");
                }
            }

            // Creation flow
            bool suggestToDoList = false;
            uiCardListScreen.OnValidateCard = () =>
            {
                if (!Statics.Data.Profile.Cards.HasCardsWithStatus(CardStatus.Completed))
                {
                    suggestToDoList = true;
                }
            };
            while (true)
            {
                if (suggestToDoList)
                {
                    // Creation flow
                    yield return new WaitForSeconds(1f);
                    Statics.Screens.GoToTodoList();
                    yield return new WaitForSeconds(0.5f);
                    yield return Statics.Screens.ShowDialog("UI/session_hint_create_card", "UI/ok");
                    suggestToDoList = false;
                }
                yield return null;
            }
        }

        private IEnumerator AssessmentFlowCO()
        {
            var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            uiTopScreen.SwitchMode(TopBarMode.Special_Assessment);

            var questionScreen = Statics.Screens.Get(ScreenID.AssessmentQuestion) as UIQuestionPopup;
            var introScreen = Statics.Screens.Get(ScreenID.AssessmentIntro) as UIAssessmentIntroScreen;
            var categoryIntroScreen = Statics.Screens.Get(ScreenID.AssessmentCategoryIntro) as UIAssessmentCategoryIntroScreen;
            var assessmentHeader = Statics.Screens.Get(ScreenID.AssessmentHeader) as UIAssessmentHeader;
            var assessmentFillbar = Statics.Screens.Get(ScreenID.AssessmentFillbar) as UIAssessmentFillbar;

            if (BlockerBG != null)
            {
                var col = BlockerBG.color;
                col.a = 1f;
                BlockerBG.colorTransition(col, 0.25f);
            }

            if (Statics.SessionFlow.IsInsideTutorial)
            {
                yield return introScreen.ShowIntro();
                while (introScreen.IsOpen)
                    yield return null;
            }

            var overallValue = 0f;
            var assessmentPercentages = new Dictionary<int, float>();
            var categories = Statics.Data.GetAll<CategoryDefinition>();
            var completionPercentage = 0f;
            var allQuestions = Statics.Data.GetAll<AssessmentQuestionDefinition>();
            assessmentFillbar.FillBar.SetValue(0, allQuestions.Count);
            assessmentFillbar.FillBar.FillImage.color = Statics.Art.UIColor.Color;
            assessmentFillbar.FillBar.BGImage.color = Statics.Art.UIColor.Color.SetSaturation(0.5f);
            yield return assessmentFillbar.OpenCO();

            var nQuestionsTotal = 0;
            foreach (var category in categories)
            {
                BlockerBG.colorTransition(category.Color.SetSaturation(0.5f), 0.25f);
                assessmentFillbar.FillBar.FillImage.color = category.Color;
                assessmentFillbar.FillBar.BGImage.color = category.Color.SetSaturation(0.5f);
                yield return assessmentHeader.ShowCategory(category);
                yield return categoryIntroScreen.ShowCategory(category);
                while (categoryIntroScreen.isActiveAndEnabled)
                    yield return null;

                var questions = allQuestions.Where(x => x.Category == category.ID).ToList();
                float totValue = 0f;
                var nQuestionsCategory = 0;
                foreach (var question in questions)
                {
#if UNITY_EDITOR
                    if (Input.GetKey(KeyCode.S))
                        break;
#endif
                    yield return questionScreen.ShowQuestion(question);
                    while (questionScreen.IsOpen)
                        yield return null;
                    var selectedAnswer = question.Answers[questionScreen.LatestSelectedOption];
                    totValue += selectedAnswer.Value;
                    nQuestionsCategory++;
                    nQuestionsTotal++;
                    assessmentFillbar.FillBar.SetValue(nQuestionsTotal, allQuestions.Count);
                    assessmentFillbar.FillBar.FillImage.color = category.Color;
                    assessmentFillbar.FillBar.BGImage.color = category.Color.SetSaturation(0.5f);
                }
                if (nQuestionsCategory == 0)
                    nQuestionsCategory = 1; // To avoid NaN
                assessmentPercentages[(int)category.ID] = totValue / nQuestionsCategory;
                overallValue += assessmentPercentages[(int)category.ID];
#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.S))
                {
                    foreach (var cd in categories)
                    {
                        assessmentPercentages[(int)cd.ID] = UnityEngine.Random.value;
                    }
                    break;
                }
#endif
            }
            yield return assessmentHeader.CloseCO();
            yield return assessmentFillbar.CloseCO();

            // 30/11/22: Recap screen is deprecated
            /*
            var assessmentRecapScreen = Statics.Screens.Get(ScreenID.AssessmentRecap) as UIAssessmentRecapPopup;
            overallValue /= categories.Count;
            assessmentRecapScreen.ShowResults(assessmentPercentages, overallValue);
            while (assessmentRecapScreen.IsOpen) yield return null;
            */

            var profileData = Statics.Data.Profile;
            foreach (var categoryData in profileData.Categories)
            {
                categoryData.AssessmentValue = assessmentPercentages[(int)categoryData.ID];
            }
            Statics.Data.SaveProfile();

            if (BlockerBG != null)
            {
                var col = BlockerBG.color;
                col.a = 0f;
                BlockerBG.colorTransition(col, 0.25f);
            }

            // Refresh pillars
            Statics.Screens.GoTo(ScreenID.Pillars);
            IsInsideAssessment = false;

            uiTopScreen.SwitchMode(TopBarMode.MainSection);
        }

        public void StopSessionMode()
        {
            SkipAssessment(false);
            if (sessionFlowCo != null)
            {
                StopCoroutine(sessionFlowCo);
                sessionFlowCo = null;
            }

            var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            uiTopScreen.SwitchMode(TopBarMode.MainSection);
            Statics.Screens.GoTo(ScreenID.Pillars);
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            uiPillarsScreen.SwitchViewMode(PillarsViewMode.Categories);
        }

        public void SkipAssessment(bool withConfirm)
        {
            StartCoroutine(SkipAssessmentCO(withConfirm));
        }

        private IEnumerator SkipAssessmentCO(bool withConfirm)
        {
            if (withConfirm)
            {
                var answer = new Ref<int>();
                yield return Statics.Screens.ShowQuestionFlow("UI/session_question_assessment_skip_title", "UI/session_question_assessment_skip_content", new[] { "UI/yes", "UI/no" }, answer);
                if (answer.Value == 1)
                    yield break;
            }

            if (assessmentCo != null)
            {
                StopCoroutine(assessmentCo);
                assessmentCo = null;
            }

            var questionScreen = Statics.Screens.Get(ScreenID.AssessmentQuestion) as UIQuestionPopup;
            var assessmentRecapScreen = Statics.Screens.Get(ScreenID.AssessmentRecap) as UIAssessmentRecapPopup;
            var introScreen = Statics.Screens.Get(ScreenID.AssessmentIntro) as UIAssessmentIntroScreen;
            var categoryIntroScreen = Statics.Screens.Get(ScreenID.AssessmentCategoryIntro) as UIAssessmentCategoryIntroScreen;
            var assessmentHeader = Statics.Screens.Get(ScreenID.AssessmentHeader) as UIAssessmentHeader;
            var assessmentFillbar = Statics.Screens.Get(ScreenID.AssessmentFillbar) as UIAssessmentFillbar;

            while (introScreen.IsOpen)
                yield return introScreen.CloseCO();
            while (questionScreen.IsOpen)
                yield return questionScreen.CloseCO();
            while (assessmentRecapScreen.IsOpen)
                yield return assessmentRecapScreen.CloseCO();
            while (categoryIntroScreen.IsOpen)
                yield return categoryIntroScreen.CloseCO();
            while (assessmentHeader.IsOpen)
                yield return assessmentHeader.CloseCO();
            while (assessmentFillbar.IsOpen)
                yield return assessmentFillbar.CloseCO();

            if (BlockerBG != null)
            {
                var col = BlockerBG.color;
                col.a = 0f;
                BlockerBG.colorTransition(col, 0.25f);
            }

            Statics.Screens.GoTo(ScreenID.Pillars);
            IsInsideAssessment = false;

            var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            uiTopScreen.SwitchMode(TopBarMode.MainSection);

        }

    }
}
