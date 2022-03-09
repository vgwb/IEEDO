using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace Ieedo
{
    public class UIHamburgerScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Debug;

        public UIButton ButtonPrefab;

        void Start()
        {
            AddButton("action_abort_activity", () => AbortActivity());

            AddButton("action_switch_session_mode", () => SwitchMode());

            AddButton("action_start_assessment", () =>
            {
                Statics.AssessmentFlow.StartAssessment();
                CloseImmediate();
            });

            AddButton("action_reset_profile", () =>
                Statics.Data.CreateNewProfile(new ProfileDescription
                {
                    Name = "TEST",
                    Country = "en",
                    Language = Language.English,
                }));

            ButtonPrefab.gameObject.SetActive(false);
        }

        private void SwitchMode()
        {
            Statics.Mode.ToggleSessionMode();
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            uiPillarsScreen.SwitchViewMode(uiPillarsScreen.ViewMode == PillarsViewMode.Categories ? PillarsViewMode.Review : PillarsViewMode.Categories);
        }

        private void AbortActivity()
        {
            Statics.ActivityFlow.CurrentActivityManager.CloseActivity(new ActivityResult(ActivityResultState.Quit, 0));
            CloseImmediate();
        }

        private void AddButton(string locKey, System.Action action)
        {
            var btn = Instantiate(ButtonPrefab, ButtonPrefab.transform.parent);
            SetupButton(btn, action);
            btn.Key = new LocalizedString("UI",locKey);
            btn.gameObject.SetActive(true);
        }
    }
}
