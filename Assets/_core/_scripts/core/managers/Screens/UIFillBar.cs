using UnityEngine;
using UnityEngine.UI;

namespace Ieedo
{
    public class UIFillBar : MonoBehaviour
    {
        public UIText PercentText;

        public Image BGImage;
        public Image FillImage;

        public void SetValue(int value, int max)
        {
            var ratio = value * 1f / max;
            PercentText.SetText($"{ratio*100}%");
            FillImage.fillAmount = ratio;
        }
    }
}