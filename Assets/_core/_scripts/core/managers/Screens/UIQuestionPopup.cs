using UnityEngine;

namespace Ieedo
{
    public class UIQuestionPopup : MonoBehaviour
    {
        public UIText Title;
        public UIText Question;

        public UIButton[] Buttons;
        public int LatestSelectedOption;

        public void ShowQuestion(AssessmentQuestionDefinition question)
        {
            Title.text = question.Category.ToString();
            Question.text = question.Question.Text;
            for (var i = 0; i < question.Answers.Length; i++)
            {
                var answer = question.Answers[i];
                Buttons[i].Text = answer.Text;
                Buttons[i].SetColor(Color.white);
                Buttons[i].gameObject.SetActive(true);

                var selectedOption = i;
                Buttons[i].OnClick.AddListener(() => SelectOption(selectedOption));
            }

            for (int i = question.Answers.Length; i < Buttons.Length; i++)
            {
                Buttons[i].gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
        }

        public System.Action<int> OnSelectOption;
        private void SelectOption(int selectedOption)
        {
            OnSelectOption?.Invoke(selectedOption);
            LatestSelectedOption = selectedOption;
            gameObject.SetActive(false);
        }
    }
}