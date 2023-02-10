using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace minigame
{
    public class ui_timer : MonoBehaviour
    {
        public TextMeshProUGUI TimeText;
        public float timeRemaining { get; private set; }
        public bool timerIsRunning { get; private set; }
        private Action OnTimerFinishCallback;

        private void Start()
        {
            timerIsRunning = false;
        }

        public void Init(int seconds, Action callback = null)
        {
            timeRemaining = seconds;
            OnTimerFinishCallback = callback;
            DisplayTime(timeRemaining);
        }

        public void StartTimer()
        {
            if (timeRemaining <= 0)
            {
                Debug.LogError("Maybe you didn't Init the timer");
            }
            timerIsRunning = true;
        }

        void Update()
        {
            if (timerIsRunning)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining > 0)
                {
                    DisplayTime(timeRemaining);
                }
                else
                {
                    timerIsRunning = false;
                    timeRemaining = 0;
                    DisplayTime(timeRemaining);
                    OnTimerFinishCallback?.Invoke();
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
