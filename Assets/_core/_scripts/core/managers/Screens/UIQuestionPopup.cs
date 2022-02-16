using System.Collections;
using UnityEngine;
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
            Title.text = question.Category.ToString();
            Question.text = question.Question.Text;
            for (var i = 0; i < question.Answers.Length; i++)
            {
                var answer = question.Answers[i];
                Buttons[i].Text = answer.Answer.Text;
                Buttons[i].SetColor(Color.white);
                Buttons[i].gameObject.SetActive(true);

                var selectedOption = i;
                SetupButton(Buttons[i], () => SelectOption(selectedOption));
            }

            for (int i = question.Answers.Length; i < Buttons.Length; i++)
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