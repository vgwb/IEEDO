using System.Collections;
using System.Linq;
using Ieedo.Utilities;
using Lean.Transition;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Ieedo
{
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        public ApplicationConfig ApplicationConfig;

        public Image LoadingObscurer;

        protected override void Init()
        {
            if (LoadingObscurer != null) LoadingObscurer.color = new Color(LoadingObscurer.color.r, LoadingObscurer.color.g, LoadingObscurer.color.b, 1f);
        }

        public IEnumerator Start()
        {
            Application.runInBackground = true;

            // Init localization
            yield return LocalizationSettings.InitializationOperation;
            var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.App.ApplicationConfig.SourceLocale);
            if (locale != null) LocalizationSettings.SelectedLocale = locale;

            // Init data
            { var _ = Statics.Data; }
            { var _ = Statics.Cards; }
            { var _ = Statics.Screens; }

            // Load the current profile
            bool canLoad = Statics.Data.LoadProfile();
            if (!canLoad)
            {
                // First time...
                Statics.Data.InitialiseCardDefinitions();

                // For now, create a new one if none is found
                Statics.Data.CreateNewProfile(new ProfileDescription()
                {
                    Name = "TEST",
                    Country = "en",
                    Language = Language.English,
                });
            }


            Statics.Screens.LoadScreens();

            // Initialise some loc data
            Statics.Score.RefreshString();

            // Load the game
            Statics.Screens.OpenImmediate(ScreenID.Top);
            Statics.Screens.OpenImmediate(ScreenID.Bottom);
            yield return Statics.Screens.TransitionToCO(ScreenID.Pillars);
            if (LoadingObscurer != null) LoadingObscurer.colorTransition(new Color(LoadingObscurer.color.r, LoadingObscurer.color.g, LoadingObscurer.color.b, 0f), 1f);
        }

    }
}
