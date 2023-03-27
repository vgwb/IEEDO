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


        void preparePage(ActivityDefinition Activity)
        {
            var gameName = $"{Activity.LocName}";
            ActivityTitle.Text.Key = new LocalizedString("Activity", $"{gameName}");
            PlaceHolderTextEvent.StringReference = new LocalizedString("Activity", $"{gameName}_prompt");

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

            ActivityData activityData = null;
            if (DebugPlay) activityData = new ActivityData();
            else activityData = Statics.Data.Profile.Activities.GetActivityData(Statics.ActivityFlow.CurrentActivity.ID);

            Pages.Clear();
            if (activityData != null && activityData.Results.Count > 0)
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

            if (Pages.Count() <= 0 || Pages[Pages.Count() - 1].Date != DateTime.Today)
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

            var topScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            topScreen.OnTargetLocaleSwitched -= HandleLocaleSwitch;
            topScreen.OnTargetLocaleSwitched += HandleLocaleSwitch;
        }

        private void HandleLocaleSwitch()
        {
            RefreshDateText();
        }

        private void RefreshDateText()
        {
            DateText.text = Pages[currentPageNumber - 1].Date.ToString("ddd dd MMM", LocalizationSettings.SelectedLocale.Formatter);
        }

        private void updateUI()
        {
            BtnNext.SetActive(Pages.Count > 1 && currentPageNumber < Pages.Count);
            BtnPrev.SetActive(Pages.Count > 1 && currentPageNumber > 1);

            RefreshDateText();
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
            var todayResult = ExtractResult();
            StartCoroutine(CompleteActivity(todayResult));
        }

        private ActivityResult ExtractResult()
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
            return todayResult;
        }

        protected override void CustomCloseActivity()
        {
            // @note: uncomment this if we want to save when we abort, too
            //var todayResult = ExtractResult();
            //Statics.ActivityFlow.RegisterResult(todayResult);
        }


        public void OnApplicationFocus(bool hasFocus)
        {
            Debug.Log("OnApplicationFocus " + hasFocus);
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log("OnApplicationPause " + pauseStatus);
            InputText.onTouchScreenKeyboardStatusChanged.AddListener(arg0 =>  Debug.LogError("Changed keyboard status: " + arg0) );
        }

    }
}
