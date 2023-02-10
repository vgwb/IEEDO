using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace minigame
{
    public class ui_timer : MonoBehaviour
    {
        public TextMeshProUGUI TimeText;
        public float timeRemaining = 60;
        public bool timerIsRunning = false;

        private void Start()
        {
            timerIsRunning = false;
        }

        public void Init(int seconds)
        {
            timeRemaining = seconds;
            DisplayTime(timeRemaining);
        }

        public void StartTimer()
        {
            timerIsRunning = true;
        }

        public void StartTimer(int seconds)
        {
            Init(seconds);
            StartTimer();
        }

        void Update()
        {
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                    Debug.Log("Time has run out!");
                    timeRemaining = 0;
                    DisplayTime(timeRemaining);
                    timerIsRunning = false;
                }
            }
        }

        void DisplayTime(float timeToDisplay)
        {
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            TimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
