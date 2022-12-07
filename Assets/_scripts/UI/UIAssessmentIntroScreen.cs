using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization;

namespace Ieedo
{
    public class UIAssessmentIntroScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.AssessmentIntro;

        public UITextContent Title;
        public UITextContent Content;

        public UIButton ContinueButton;

        public IEnumerator ShowIntro()
        {
            Title.Text.Key = new LocalizedString("UI", "assessment_intro_title");
            Content.Text.Key = new LocalizedString("UI", "assessment_intro_content");

            SetupButton(ContinueButton, Close);
            yield return OpenCO();
        }

    }
}
