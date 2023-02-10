using System;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;

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
        public UITextContent ActivityTitle;
        public LocalizeStringEvent PlaceHolderTextEvent;
        public TMP_Text DateText;
        public TMP_Text PageText;
        public TMP_InputField InputText;
        public GameObject BtnNext;
        public GameObject BtnPrev;

        public Image Background;
        public Sprite TextureDiary;
        public Sprite TextureLetter;

        public List<Page> Pages = new List<Page>();
        private int currentPageNumber;
        private int totalPageNumber;

        void Start()
        {
            if (DebugAutoplay)
            {
                Debug.Log("AUTOPLAY START");
                preparePage(Activity);
            }
        }

        void preparePage(ActivityDefinition Activity)
        {
            var gameName = $"{Activity.LocName}";
            ActivityTitle.Text.Key = new LocalizedString("Activity", $"{gameName}");
            PlaceHolderTextEvent.StringReference = new LocalizedString("Activity", $"{gameName}_description");

            if (Activity.ID == ActivityID.Write_Diary)
            {
                Background.sprite = TextureDiary;
            }
            else
            {
                Background.sprite = TextureLetter;
            }
        }

        protected override void SetupActivity(int currentLevel)
        {
            preparePage(Statics.ActivityFlow.CurrentActivity);
            var activityData = Statics.Data.Profile.Activities.GetActivityData(Statics.ActivityFlow.CurrentActivity.ID);

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

            if (Pages.Count() > 0 && Pages[Pages.Count() - 1].Date == DateTime.Today)
            {

            }
            else
            {
                Pages.Add(new Page
                {
                    Text = "",
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

            DateText.text = Pages[currentPageNumber - 1].Date.ToString("ddd dd MMM", LocalizationSettings.SelectedLocale.Formatter);
            InputText.text = Pages[currentPageNumber - 1].Text;
            if (totalPageNumber > 1)
            {
                PageText.text = currentPageNumber + "\n-\n" + totalPageNumber;
            }
            else
            {
                PageText.text = "";
            }

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
            var activityData = Statics.Data.Profile.Activities.GetActivityData(Statics.ActivityFlow.CurrentActivity.ID);
            var todayResult = activityData.Results.FirstOrDefault(x => x.Timestamp.Equals(Timestamp.Today));
            if (todayResult == null)
            {
                todayResult = new ActivityResult(ActivityResultState.Win, Activity.PointsOnWin);
                todayResult.Timestamp = Timestamp.Today;
            }
            todayResult.CustomData = todayText;
            StartCoroutine(CompleteActivity(todayResult));
        }

        protected override void CustomCloseActivity()
        {
            // Debug.Log("CustomCloseActivity");
            CompleteActivity();
        }

    }
}
