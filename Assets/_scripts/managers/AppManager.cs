using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Lean.Transition;
using Ieedo.Utilities;

namespace Ieedo
{
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        public ApplicationConfig ApplicationConfig;
        public Image LoadingObscurer;

        protected override void Init()
        {
            if (LoadingObscurer != null)
                LoadingObscurer.color = new Color(LoadingObscurer.color.r, LoadingObscurer.color.g, LoadingObscurer.color.b, 1f);
        }

        public IEnumerator Start()
        {
            Application.runInBackground = true;

            // Init localization
            yield return LocalizationSettings.InitializationOperation;

            // Init data
            { var _ = Statics.Data; }
            { var _ = Statics.Cards; }
            { var _ = Statics.Screens; }

            Statics.Screens.LoadScreens();

            // Load the current profile
            bool canLoad = Statics.Data.LoadProfile();
            if (!canLoad)
            {
                // First time...
                Statics.Data.InitialiseCardDefinitions();

                // For now, create a new one if none is found
                Statics.Data.CreateDefaultNewProfile();
            }

            if (ApplicationConfig.ResetProfileAtVersionMismatch)
            {
                // Reset profile at the new version (for now)
                if (Statics.Data.Profile.Version != ApplicationConfig.Version)
                {
                    Debug.LogWarning("Profile version mismatch! We reset to a new profile");
                    Statics.Data.CreateDefaultNewProfile();
                }
            }

            // Setup correct locale based on the player profile
            var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.Data.Profile.Settings.HostCountryLocale);
            if (locale != null)
                LocalizationSettings.SelectedLocale = locale;

            // Initialise UI
            Statics.Points.RefreshPointsText();

            // Load the game
            Statics.Screens.OpenImmediate(ScreenID.Top);
            Statics.Screens.OpenImmediate(ScreenID.Bottom);
            yield return Statics.Screens.TransitionToCO(ScreenID.Pillars);
            if (LoadingObscurer != null)
            {
                LoadingObscurer.colorTransition(new Color(LoadingObscurer.color.r, LoadingObscurer.color.g, LoadingObscurer.color.b, 0f), 1f);
            }

            if (Statics.Data.Profile.Settings.TutorialNotCompleted)
            {
                yield return HandleTutorial();
            }
        }


        // TODO: move to OnboardingManager / TutorialManager?
        public IEnumerator HandleTutorial()
        {
            Statics.Mode.SetSessionMode(SessionMode.Solo);

            Statics.SessionFlow.IsInsideTutorial = true;
            var topScreen = Statics.Screens.Get(ScreenID.Top) as UITopScreen;
            var botScreen = Statics.Screens.Get(ScreenID.Bottom) as UIBottomScreen;
            var cardListScreen = Statics.Screens.Get(ScreenID.CardList) as UICardListScreen;
            var dialogPopup = Statics.Screens.Get(ScreenID.Dialog) as UIDialogPopup;
            var pillarsScreen = Statics.Screens.Get(ScreenID.Pillars) as UIPillarsScreen;

            Statics.Points.CurrentPointsText.transform.parent.gameObject.SetActive(false);
            topScreen.HamburgerButton.Hide();
            topScreen.SessionModeButton.Hide();
            topScreen.InstantTranslationButton.Hide();
            pillarsScreen.SwitchViewButton.Hide();

            botScreen.btnActivities.enabled = false;
            botScreen.btnPillars.enabled = false;

            yield return ProfileCreationFlow();
            yield return Statics.Screens.ShowDialog("UI/intro_content_1", "UI/ok");

            // Instant translation tutorial
            dialogPopup.Button.gameObject.SetActive(false);
            bool passed = false;
            void Pass() => passed = true;
            topScreen.OnTargetLocaleSwitched += Pass;
            yield return Statics.Screens.ShowDialog("UI/intro_instant_translation", "UI/ok", waitForClosing: false);
            topScreen.InstantTranslationButton.gameObject.SetActive(true);
            Statics.Tutorial.ShowTutorialArrowOn(topScreen.InstantTranslationButton.gameObject, -20f);
            while (!passed)
                yield return null;
            topScreen.OnTargetLocaleSwitched -= Pass;
            Statics.Tutorial.HideTutorialArrow();
            dialogPopup.Button.gameObject.SetActive(true);
            while (Statics.Screens.Get(ScreenID.Dialog).IsOpen)
                yield return null;

            // Session mode tutorial
            passed = false;
            dialogPopup.Button.gameObject.SetActive(false);
            topScreen.OnSessionModeToggled += Pass;
            yield return Statics.Screens.ShowDialog("UI/intro_content_2", "UI/ok", waitForClosing: false);
            topScreen.SessionModeButton.gameObject.SetActive(true);
            Statics.Tutorial.ShowTutorialArrowOn(topScreen.SessionModeButton.gameObject, 20f);
            Statics.SessionFlow.IsInsideAssessment = true; // Force assessment on, so we make sure we do not advance the tutorial too early
            while (!passed)
                yield return null;
            Statics.Tutorial.HideTutorialArrow();
            topScreen.OnSessionModeToggled -= Pass;
            dialogPopup.Button.gameObject.SetActive(true);
            yield return dialogPopup.CloseCO();
            //while (dialogPopup.IsOpen) yield return null;
            //yield return Statics.Screens.ShowDialog("UI/intro_content_2", "UI/start_session");

            Statics.Data.Profile.Settings.TutorialNotCompleted = false;
            Statics.Data.SaveProfile();

            // Go directly to the session mode
            //Statics.Mode.ToggleSessionMode();
            // Wait for the assessment to end...
            while (Statics.SessionFlow.IsInsideAssessment)
                yield return null;

            // Go to Cards list tutorial
            passed = false;
            botScreen.OnCardsClicked += Pass;
            Statics.Tutorial.ShowTutorialArrowOn(botScreen.btnCards.gameObject, 160f);
            while (!passed)
                yield return null;
            botScreen.OnCardsClicked -= Pass;
            Statics.Tutorial.HideTutorialArrow();

            // Create a card tutorial
            passed = false;
            cardListScreen.OnCreateClicked += Pass;
            Statics.Tutorial.ShowTutorialArrowOn(cardListScreen.CreateCardButton.gameObject, 20f);
            while (!passed)
                yield return null;
            cardListScreen.OnCreateClicked -= Pass;
            Statics.Tutorial.HideTutorialArrow();

            // Tutorial end
            Statics.Points.CurrentPointsText.transform.parent.gameObject.SetActive(true);
            topScreen.HamburgerButton.Show();
            topScreen.SessionModeButton.Show();
            topScreen.InstantTranslationButton.Show();
            pillarsScreen.SwitchViewButton.Show();

            botScreen.btnActivities.enabled = true;
            botScreen.btnPillars.enabled = true;

            Statics.SessionFlow.IsInsideTutorial = false;
        }

        private IEnumerator ProfileCreationFlow()
        {
            Statics.Data.CreateDefaultNewProfile();
            var languageScreen = Statics.Screens.Get(ScreenID.LanguageSelection) as UILanguageSelectionScreen;
            var countryScreen = Statics.Screens.Get(ScreenID.CountrySelection) as UICountrySelectionScreen;

            languageScreen.ButtonsSelection.HideButtons();
            yield return Statics.Screens.OpenCO(ScreenID.LanguageSelection);
            yield return languageScreen.PerformSelection(Statics.Data.Profile);
            countryScreen.ButtonsSelection.HideButtons();
            yield return Statics.Screens.CloseOpenCO(ScreenID.LanguageSelection, ScreenID.CountrySelection);
            yield return countryScreen.PerformSelection(Statics.Data.Profile);
            yield return Statics.Screens.CloseCO(ScreenID.CountrySelection);

        }
    }
}
