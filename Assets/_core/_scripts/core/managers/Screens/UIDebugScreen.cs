namespace Ieedo
{
    public class UIDebugScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Debug;

        public UIButton ButtonPrefab;

        public UIButton CloseButton;

        void Start()
        {
            AddButton("Reset", () =>
                Statics.Data.CreateNewProfile(new ProfileDescription
                {
                    Name = "TEST",
                    Country = "en",
                    Language = Language.English,
                }));

            SetupButton(CloseButton, CloseImmediate);
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