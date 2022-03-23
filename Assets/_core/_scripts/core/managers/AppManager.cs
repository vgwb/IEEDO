using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ieedo.Utilities;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace Ieedo
{
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        public ApplicationConfig ApplicationConfig;


        public IEnumerator Start()
        {
            // Init localization
            //Debug.LogError("Wait for localization...");
            yield return LocalizationSettings.InitializationOperation;
            var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code == Statics.App.ApplicationConfig.SourceLocale);
            if (locale != null) LocalizationSettings.SelectedLocale = locale;
            //Debug.LogError(LocalizationSettings.SelectedLocale.LocaleName);

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

            // Load the game
            Statics.Screens.OpenImmediate(ScreenID.Top);
            Statics.Screens.OpenImmediate(ScreenID.Bottom);
            yield return Statics.Screens.TransitionToCO(ScreenID.Pillars);
        }

    }
}
