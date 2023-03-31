using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using Lean.Gui;

namespace Ieedo
{
    public class UIHamburgerScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Hamburger;

        public UIText AppVersionText;
        public LeanToggle SfxToggle;
        public LeanToggle NotificationsToggle;

        public RectTransform CheatSection;
        public RectTransform SettingsSection;
        public RectTransform CreditsSection;

        public UIButton btnHiddenCheats;
        private int nCheatEnterCounter;

        public UIButton ButtonPrefab;

        private UIButton btnAbortSpecialSection;
        private UIButton btnSkipAssessment;
        private UIButton btnSwitchSessionMode;
        private UIButton btnGenerateTestPillars;
        private UIButton btnGenerateTestCards;
        private UIButton btnTestResetScore;
        private UIButton btnTestIncreaseScore;
        private UIButton btnTestDecreaseScore;
        private UIButton btnTestAddDiary;
        private UIButton btnTestNotifications;
        private UIButton btnChangeLanguage;


        private bool initialised = false;
        void Init()
        {
            if (initialised)
                return;
            initialised = true;

            SfxToggle.On = !Statics.Data.Profile.Settings.SfxDisabled;
            NotificationsToggle.On = !Statics.Data.Profile.Settings.NotificationsDisabled;
            AppVersionText.SetTextRaw("Version v0." + Statics.App.ApplicationConfig.Version.ToString());

            CheatSection.gameObject.SetActive(false);
            SetupButton(btnHiddenCheats, () =>
            {
                nCheatEnterCounter++;
                if (nCheatEnterCounter >= 5)
                {
                    CheatSection.gameObject.SetActive(true);
                }
            });

            // Debug buttons
            btnAbortSpecialSection = AddUnlocalizedButton("Abort activity", () =>
            {
                AppManager.I.StartCoroutine(AbortSpecialSectionCO());
                CloseImmediate();
            });

            btnSkipAssessment = AddUnlocalizedButton("Skip assessment", () =>
            {
                Statics.SessionFlow.SkipAssessment(true);
                CloseImmediate();
            });

            btnGenerateTestPillars = AddUnlocalizedButton("Generate test pillars", () =>
            {
                var profileData = Statics.Data.Profile;
                foreach (var categoryData in profileData.Categories)
                    categoryData.AssessmentValue = UnityEngine.Random.value;
                Statics.Data.SaveProfile();
                Log.Warn($"Generate random assessment data");
                Statics.Screens.GoTo(ScreenID.Pillars);
            });

            btnGenerateTestCards = AddUnlocalizedButton("Generate 5 test cards", () =>
            {
                Statics.Cards.GenerateTestCards(5);
            });

            btnTestIncreaseScore = AddUnlocalizedButton("Add 100 points", () =>
            {
                Statics.Points.AddPoints(100);
            });

            btnTestDecreaseScore = AddUnlocalizedButton("Remove 100 points", () =>
            {
                Statics.Points.AddPoints(-100);
            });

            btnTestResetScore = AddUnlocalizedButton("Reset Score", () =>
            {
                Statics.Points.Reset();
            });

            btnTestAddDiary = AddUnlocalizedButton("Add Diary Pages", () =>
            {
                for (int i = 4; i >= 0; i--)
                {
                    var res = new ActivityResult
                    {
                        Result = ActivityResultState.Win,
                        Points = 60,
                        Timestamp = new Timestamp(DateTime.Today + TimeSpan.FromDays(-i)),
                        CustomData = $"Cheat added text for diary. Added with index {i}"
                    };
                    Statics.ActivityFlow.RegisterResult(res, ActivityID.Write_Diary);
                }
                CloseImmediate();
            });

            btnTestNotifications = AddUnlocalizedButton("Test Notifications", () =>
            {
                Statics.Notifications.TestLocalNotification();
            });

            btnChangeLanguage = AddUnlocalizedButton("Change language", () =>
            {
                AppManager.I.StartCoroutine(ChangeLanguageCO());
                CloseImmediate();
            });

            ButtonPrefab.gameObject.SetActive(false);
        }

        protected override IEnumerator OnPreOpen()
        {
            Init();
            btnAbortSpecialSection.gameObject.SetActive(Statics.ActivityFlow.IsInsideActivity);
            btnSkipAssessment.gameObject.SetActive(Statics.SessionFlow.IsInsideAssessment);
            btnGenerateTestCards.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            btnGenerateTestPillars.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            btnTestIncreaseScore.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            btnTestAddDiary.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            yield break;
        }

        private IEnumerator ResetProfileCO()
        {
            var answer = new Ref<int>();
            yield return Statics.Screens.ShowQuestionFlow("UI/reset_profile_title", "UI/reset_profile_question", new[] { "UI/yes", "UI/no" }, answer);
            if (answer.Value == 0)
            {
                Statics.Data.CreateDefaultNewProfile();
                yield return AppManager.I.HandleTutorial();
            }
        }

        private IEnumerator ChangeLanguageCO()
        {
            yield return AppManager.I.LanguageChangeFlow();
        }


        public IEnumerator AbortSpecialSectionCO()
        {
            var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            switch (uiTopScreen.Mode)
            {
                case TopBarMode.MainSection:
                    break;
                case TopBarMode.Special_Assessment:
                {
                    Statics.SessionFlow.SkipAssessment(true);
                }
                break;
                case TopBarMode.Special_Activity:
                {
                    var answer = new Ref<int>();
                    yield return Statics.Screens.ShowQuestionFlow("Activity/activity_abort_title", "Activity/activity_abort_question", new[] { "UI/yes", "UI/no" }, answer);
                    if (answer.Value == 0)
                    {
                        Statics.ActivityFlow.CurrentActivityManager.CloseActivity();
                    }
                }
                break;
                case TopBarMode.Special_CardCreation:
                {
                    var uiCardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
                    AppManager.I.StartCoroutine(uiCardListScreen.CreationAbortCO());
                }
                break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private UIButton AddButton(string locKey, System.Action action, RectTransform parent = null)
        {
            if (parent == null)
                parent = ButtonPrefab.transform.parent as RectTransform;
            var btn = Instantiate(ButtonPrefab, parent);
            SetupButton(btn, action);
            btn.Key = new LocalizedString("UI", locKey);
            btn.gameObject.SetActive(true);
            return btn;
        }

        private UIButton AddUnlocalizedButton(string text, System.Action action, RectTransform parent = null)
        {
            if (parent == null)
                parent = ButtonPrefab.transform.parent as RectTransform;
            ;
            var btn = Instantiate(ButtonPrefab, parent);
            SetupButton(btn, action);
            btn.Text = text;
            btn.gameObject.SetActive(true);
            return btn;
        }

        public void OnBtnResetProfile()
        {
            AppManager.I.StartCoroutine(ResetProfileCO());
            CloseHamburgerMenu();
        }

        private void CloseHamburgerMenu()
        {
            CloseImmediate();
            var uiTopScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            uiTopScreen.SetHamburgerIcon();
        }

        public void OnOpenHelpWebsite()
        {
            Debug.Log("OnOpenHelpWebsite");
            Application.OpenURL(Statics.App.ApplicationConfig.UrlSupportWebsite);
        }

        public void OnSetSfx(bool _sfxOn)
        {
            Debug.Log("Sfx " + _sfxOn);
            Statics.Data.Profile.Settings.SfxDisabled = !_sfxOn;
            Statics.Data.SaveProfile();
        }

        public void OnSetNotifications(bool _notificationsOn)
        {
            Debug.Log("OnSetNotifications " + _notificationsOn);
            Statics.Data.Profile.Settings.NotificationsDisabled = !_notificationsOn;
            Statics.Data.SaveProfile();
        }

    }
}
