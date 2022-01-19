using Lean.Gui;

namespace Ieedo
{
    public class UIIntroScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Intro;

        public LeanButton btn1;
        public LeanButton btn2;
        public LeanButton btn3;

        void Start()
        {
            btn1.OnClick.AddListener(() => GoTo(ScreenID.CardEditor));
            btn2.OnClick.AddListener(() => GoTo(ScreenID.Assessment));
            btn3.OnClick.AddListener(() => GoTo(ScreenID.History));
        }
    }
}