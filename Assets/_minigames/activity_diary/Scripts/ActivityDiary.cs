using System;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace Ieedo.games.diary
{
    public class Page
    {
        public string Text;
        public DateTime Date;
    }

    public class ActivityDiary : ActivityManager
    {
        public TMP_Text DateText;
        public TMP_Text PageText;
        public TMP_InputField InputText;
        public GameObject BtnNext;
        public GameObject BtnPrev;

        public List<Page> Pages = new List<Page>();
        private int currentPageNumber;
        private int totalPageNumber;

        protected override void SetupActivity(int currentLevel)
        {
            //            Debug.Log($"Starting ActivityDiary");

            var activityData = Statics.Data.Profile.ActivitiesData.GetActivityData(ActivityID.Write_Diary);

            Pages.Clear();
            if (activityData.Results.Count > 0)
            {
                foreach (ActivityResult activityResult in activityData.Results)
                {
                    Pages.Add(new Page
                    {
                        Text = activityResult.CustomData,
                        Date = activityResult.Timestamp.Date,
                    });
                }
            }
            else
            {
                Pages.Add(new Page
                {
                    Text = "write",
                    Date = DateTime.Today,
                });
            }

            currentPageNumber = Pages.Count();
            totalPageNumber = Pages.Count();
            updateUI();
        }

        private void updateUI()
        {
            BtnNext.SetActive(Pages.Count > 1 && currentPageNumber < Pages.Count);
            BtnPrev.SetActive(Pages.Count > 1 && currentPageNumber > 1);

            DateText.text = Pages[currentPageNumber - 1].Date.ToString("dd-MM-yyyy");
            InputText.text = Pages[currentPageNumber - 1].Text;
            PageText.text = currentPageNumber + " / " + totalPageNumber;

            if (currentPageNumber == totalPageNumber)
            {
                InputText.interactable = true;
            }
            else
            {
                InputText.interactable = false;
            }
        }

        public void OnBtnDone()
        {
            CompleteActivity();
        }

        public void OnBtnNext()
        {
            if (currentPageNumber < Pages.Count)
            {
                currentPageNumber++;
                updateUI();
            }
        }

        public void OnBtnPrev()
        {
            if (currentPageNumber > 1)
            {
                currentPageNumber--;
                updateUI();
            }
        }

        public void CompleteActivity()
        {
            var todayText = InputText.text;
            //            Debug.LogError("Updating page of " + Timestamp.Today.Date + " with text " + todayText);
            var activityData = Statics.Data.Profile.ActivitiesData.GetActivityData(ActivityID.Write_Diary);
            var todayResult = activityData.Results.FirstOrDefault(x => x.Timestamp.Equals(Timestamp.Today));
            if (todayResult == null)
            {
                todayResult = new ActivityResult(ActivityResultState.Win, 10);
                todayResult.Timestamp = Timestamp.Today;
            }
            todayResult.CustomData = todayText;
            StartCoroutine(CompleteActivity(todayResult));
        }

    }
}
