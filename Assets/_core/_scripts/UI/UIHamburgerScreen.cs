using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace Ieedo
{
    public class UIHamburgerScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Hamburger;

        public UIButton ButtonPrefab;

        private UIButton btnAbort;
        private UIButton btnSwitchSessionMode;
        private UIButton btnStartAssessment;
        private UIButton btnGenerateTestPillars;
        private UIButton btnGenerateTestCards;
        private UIButton btnTestIncreaseScore;
        private UIButton btnResetProfile;

        private bool initialised = false;
        void Init()
        {
            if (initialised) return;
            initialised = true;
            btnAbort = AddButton("action_abort_activity", () =>
            {
                AbortActivity();
                CloseImmediate();
            });

            //btnSwitchSessionMode = AddButton("action_switch_session_mode", () => SwitchMode());

            btnStartAssessment = AddButton("action_start_assessment", () =>
            {
                Statics.AssessmentFlow.StartAssessment();
                CloseImmediate();
            });

            btnGenerateTestPillars = AddButton("action_generate_test_pillars", () =>
            {
                var profileData = Statics.Data.Profile;
                foreach (var categoryData in profileData.Categories)
                    categoryData.AssessmentValue = UnityEngine.Random.value;
                Statics.Data.SaveProfile();
                Log.Err($"Generate random assessment data");
                GoTo(ScreenID.Pillars);
            });

            btnGenerateTestCards = AddButton("action_generate_test_cards", () =>
            {
                Statics.Cards.GenerateTestCards(5);
            });

            btnTestIncreaseScore = AddButton("action_test_increase_score", () =>
            {
                Statics.Score.AddScore(100);
            });

            btnResetProfile =  AddButton("action_reset_profile", () =>
            {
                Statics.Data.CreateNewProfile(new ProfileDescription
                {
                    Name = "TEST",
                    Country = "en",
                    Language = Language.English,
                });
                CloseImmediate();
            });

            ButtonPrefab.gameObject.SetActive(false);
        }

        protected override IEnumerator OnPreOpen()
        {
            Init();
            btnAbort.gameObject.SetActive(Statics.ActivityFlow.IsInsideActivity);
            btnGenerateTestCards.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            btnGenerateTestPillars.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            btnTestIncreaseScore.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            btnStartAssessment.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            btnResetProfile.gameObject.SetActive(!Statics.ActivityFlow.IsInsideActivity);
            yield break;
        }


        private void AbortActivity()
        {
            Statics.ActivityFlow.CurrentActivityManager.CloseActivity(new ActivityResult(ActivityResultState.Quit, 0));
            CloseImmediate();
        }

        private UIButton AddButton(string locKey, System.Action action)
        {
            var btn = Instantiate(ButtonPrefab, ButtonPrefab.transform.parent);
            SetupButton(btn, action);
            btn.Key = new LocalizedString("UI",locKey);
            btn.gameObject.SetActive(true);
            return btn;
        }
    }
}
