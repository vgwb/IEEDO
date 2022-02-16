namespace Ieedo
{
    public class UIDebugScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Debug;

        public UIButton ButtonPrefab;
        void Start()
        {
            AddButton("Reset", () =>
                Statics.Data.CreateNewProfile(new ProfileDescription
                {
                    Name = "TEST",
                    Country = "en",
                    Language = Language.English,
                }));
        }

        private void AddButton(string text, System.Action action)
        {
            var btn = Instantiate(ButtonPrefab);
            SetupButton(btn, action);
            btn.Text = text;
        }
    }
}