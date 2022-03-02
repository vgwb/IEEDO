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
            TitleBG.color = category.Color;
            QuestionBG.color = category.Color.SetValue(0.5f).SetSaturation(0.5f);
            var answers = question.Answers.Select(x => x.Answer.Key).ToArray();
            yield return ShowQuestion(category.Title.Key, question.Question.Key, answers);
        }

        public IEnumerator ShowQuestion(LocalizedString title, LocalizedString question, LocalizedString[] answers)
        {
            Title.Key = title;
            Question.Key = question;
            for (var i = 0; i < answers.Length; i++)
            {
                Buttons[i].Key = answers[i];
                Buttons[i].gameObject.SetActive(true);

                var selectedOption = i;
                SetupButton(Buttons[i], () => SelectOption(selectedOption));
            }

            for (int i = answers.Length; i < Buttons.Length; i++)
            {
                Buttons[i].gameObject.SetActive(false);
            }

            yield return OpenCO();
        }

        public System.Action<int> OnSelectOption;
        private void SelectOption(int selectedOption)
        {
            OnSelectOption?.Invoke(selectedOption);
            LatestSelectedOption = selectedOption;
            Close();
        }
    }
}
