namespace Ieedo
{
    public class UIHamburgerScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Debug;

        public UIButton ButtonPrefab;

        public UIButton CloseButton;

        void Start()
        {

            AddButton("Switch Mode", () => SwitchMode());

            AddButton("DEBUG Assessment", () => Statics.AssessmentFlow.StartAssessment());

            AddButton("DEBUG Reset Profile", () =>
                Statics.Data.CreateNewProfile(new ProfileDescription
                {
                    Name = "TEST",
                    Country = "en",
                    Language = Language.English,
                }));

            SetupButton(CloseButton, CloseImmediate);

            ButtonPrefab.gameObject.SetActive(false);
        }

        private void SwitchMode()
        {
            Statics.Mode.ToggleMode();
            var uiPillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;
            uiPillarsScreen.SwitchViewMode(uiPillarsScreen.ViewMode == PillarsViewMode.Categories ? PillarsViewMode.Review : PillarsViewMode.Categories);
        }

        private void AddButton(string text, System.Action action)
        {
            var btn = Instantiate(ButtonPrefab, ButtonPrefab.transform.parent);
            SetupButton(btn, action);
            btn.Text = text;
            btn.gameObject.SetActive(true);
        }
    }
}