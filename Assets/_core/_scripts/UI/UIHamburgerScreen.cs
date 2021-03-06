using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Lean.Gui;

namespace Ieedo
{
    public class UIHamburgerScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Hamburger;

        public LeanToggle SfxToggle;

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
        private UIButton btnTestIncreaseScore;
        private UIButton btnResetProfile;
        private UIButton btnTestAddDiary;


        private bool initialised = false;
        void Init()
        {
            if (initialised)
                return;
            initialised = true;

            SfxToggle.On = !Statics.Data.Profile.Description.SfxDisabled;

            CheatSection.gameObject.SetActive(false);
            SetupButton(btnHiddenCheats, () => {
                nCheatEnterCounter++;
                if (nCheatEnterCounter >= 5)
                {
                    CheatSection.gameObject.SetActive(true);
                }
            });

            // Debug buttons
            btnAbortSpecialSection = AddButton("action_abort_activity", () =>
            {
                AppManager.I.StartCoroutine(AbortSpecialSectionCO());
                CloseImmediate();
            });

            btnSkipAssessment = AddButton("action_skip_assessment", () =>
            {
                Statics.SessionFlow.SkipAssessment();
                CloseImmediate();
            });

            btnGenerateTestPillars = AddButton("action_generate_test_pillars", () =>
            {
                var profileData = Statics.Data.Profile;
                foreach (var categoryData in profileData.Categories)
                    categoryData.AssessmentValue = UnityEngine.Random.value;
                Statics.Data.SaveProfile();
                Log.Err($"Generate random assessment data");
                Statics.Screens.GoTo(ScreenID.Pillars);
            });

            btnGenerateTestCards = AddButton("action_generate_test_cards", () =>
            {
                Statics.Cards.GenerateTestCards(5);
            });

            btnTestIncreaseScore = AddButton("action_test_increase_score", () =>
            {
                Statics.Score.AddScore(100);
            });

            btnResetProfile = AddButton("action_reset_profile", () =>
            {
                AppManager.I.StartCoroutine(ResetProfileCO());
                CloseImmediate();
            },SettingsSection);

            btnTestAddDiary = AddUnlocalizedButton("Add Diary Pages", () =>
            {
                for (int i = 4; i >= 0; i--)
                {
                    var res = new ActivityResult
                    {
                        Result = ActivityResultState.Win,
                        Score = 10,
                        Timestamp = new Timestamp(DateTime.Today + TimeSpan.FromDays(-i)),
                        CustomData = $"Cheat added text for diary. Added with index {i}"
                    };
                    Statics.ActivityFlow.RegisterResult(res, ActivityID.Diary);
                }
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
            btnResetProfile.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            btnTestAddDiary.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            yield break;
        }

        private IEnumerator ResetProfileCO()
        {
            var answer = new Ref<int>();
            yield return Statics.Screens.ShowQuestionFlow("UI/reset_profile_title", "UI/reset_profile_question", new[] { "UI/yes", "UI/no" }, answer);
            if (answer.Value == 0)
            {
                Statics.Data.CreateNewProfile(new ProfileDescription
                {
                    Name = "TEST",
                    Country = "en",
                    Language = Language.English,
                    IsNewProfile = true
                });
                yield return AppManager.I.HandleNewProfileStart();
            }
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
                    Statics.SessionFlow.SkipAssessment();
                }
                    break;
                case TopBarMode.Special_Activity:
                {
                    var answer = new Ref<int>();
                    yield return Statics.Screens.ShowQuestionFlow("UI/activity_abort_title", "UI/activity_abort_question", new[] { "UI/yes", "UI/no" }, answer);
                    if (answer.Value == 0)
                    {
                        Statics.ActivityFlow.CurrentActivityManager.CloseActivity(new ActivityResult(ActivityResultState.Quit, 0));
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
            if (parent == null) parent = ButtonPrefab.transform.parent as RectTransform;
            var btn = Instantiate(ButtonPrefab, parent);
            SetupButton(btn, action);
            btn.Key = new LocalizedString("UI", locKey);
            btn.gameObject.SetActive(true);
            return btn;
        }

        private UIButton AddUnlocalizedButton(string text, System.Action action, RectTransform parent = null)
        {
            if (parent == null) parent = ButtonPrefab.transform.parent as RectTransform;;
            var btn = Instantiate(ButtonPrefab, parent);
            SetupButton(btn, action);
            btn.Text = text;
            btn.gameObject.SetActive(true);
            return btn;
        }

        public void OnSetSfx(bool _sfxOn)
        {
            Debug.Log("Sfx " + _sfxOn);
            Statics.Data.Profile.Description.SfxDisabled = !_sfxOn;
            Statics.Data.SaveProfile();
        }

    }
}
