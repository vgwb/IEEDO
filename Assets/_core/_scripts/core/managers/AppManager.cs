using System;
using System.Collections;
using System.Collections.Generic;
using Ieedo.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ieedo
{
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        public ApplicationConfig ApplicationConfig;


        public IEnumerator Start()
        {
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


        #region Activity Flow

        public ActivityDefinition CurrentActivity;

        public void LaunchActivity(ActivityID activity)
        {
            CurrentActivity = Statics.Data.Get<ActivityDefinition>((int)activity);
            SceneManager.LoadScene(CurrentActivity.SceneName, LoadSceneMode.Additive);
        }

        public void CloseActivity()
        {
            if (CurrentActivity)
            {
                SceneManager.UnloadSceneAsync(CurrentActivity.SceneName);
            }
            CurrentActivity = null;
        }

        #endregion
    }
}