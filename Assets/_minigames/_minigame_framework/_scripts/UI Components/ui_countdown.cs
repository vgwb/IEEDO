using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace minigame
{
    public class ui_countdown : MonoBehaviour
    {
        public TextMeshProUGUI CounterText;

        public void Show(int value)
        {
            CounterText.text = value.ToString();
        }

    }
}
