using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Ieedo
{
    public class UIQuestionPopup : UIScreen
    {
        public override ScreenID ID => ScreenID.Question;

        public UIText Title;
        public UIText Question;

        public UIButton[] Buttons;
        public int LatestSelectedOption;

        public Image TitleBG;
        public Image QuestionBG;

        public IEnumerator ShowQuestion(AssessmentQuestionDefinition question)
        {
            var category = Statics.Data.Get<CategoryDefinition>((int)question.Category);
            var answers = question.Answers.Select(x => x.Answer.Key).ToArray();
            yield return ShowQuestion(category.Title.Key, question.Question.Key, answers, category.Color);
        }

        public IEnumerator ShowQuestionFlow(string title, string question, string[] answers, Ref<int> selectedAnswer)
        {
            yield return ShowQuestion(title, question, answers);
            while (IsOpen)
                yield return null;
            selectedAnswer.Value = LatestSelectedOption;
        }

        public IEnumerator ShowQuestionFlow(LocalizedString title, LocalizedString question, LocalizedString[] answers, Ref<int> selectedAnswer)
        {
            yield return ShowQuestion(title, question, answers);
            while (IsOpen)
                yield return null;
            selectedAnswer.Value = LatestSelectedOption;
        }

        public IEnumerator ShowQuestion(string title, string question, string[] answers)
        {
            var answerKeys = new LocalizedString[answers.Length];
            for (int i = 0; i < answers.Length; i++)
                answerKeys[i] = LocString.FromStr(answers[i]);
            yield return ShowQuestion(LocString.FromStr(title), LocString.FromStr(question), answerKeys);
        }

        public IEnumerator ShowQuestion(LocalizedString title, LocalizedString question, LocalizedString[] answers, Color col = default)
        {
            if (col == default)
            {
                TitleBG.color = Statics.Art.ToTitle(Statics.Art.UIColor.Color);
                QuestionBG.color = Statics.Art.ToBG(Statics.Art.UIColor.Color);
            }
            else
            {
                TitleBG.color = Statics.Art.ToTitle(col);
                QuestionBG.color = Statics.Art.ToBG(col);
            }


            Title.Key = title;
            Question.Key = question;
            for (var i = 0; i < answers.Length; i++)
            {
                Buttons[i].Key = answers[i];
                Buttons[i].gameObject.SetActive(true);
                Buttons[i].transform.localScale = Vector3.zero;

                var selectedOption = i;
                SetupButton(Buttons[i], () => SelectOption(selectedOption));
            }

            for (int i = answers.Length; i < Buttons.Length; i++)
            {
                Buttons[i].gameObject.SetActive(false);
            }

            yield return OpenCO();

            for (var i = 0; i < answers.Length; i++)
            {
                Buttons[i].AnimateAppear();
            }

        }

        public System.Action<int> OnSelectOption;
        private void SelectOption(int selectedOption)
        {
            SoundManager.I.PlaySfx(SfxEnum.click);
            OnSelectOption?.Invoke(selectedOption);
            LatestSelectedOption = selectedOption;
            Close();
        }

    }
}
