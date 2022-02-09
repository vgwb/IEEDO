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

        public ActivityDefinition CurrentActivity;

        public IEnumerator Start()
        {
            // Init data
            { var _ = Statics.Data; }
            { var _ = Statics.Cards; }
            { var _ = Statics.Screens; }

            // Load the current profile
            Statics.Data.LoadProfile();

            Statics.Screens.LoadScreens();

            // Load the game
            Statics.Screens.OpenImmediate(ScreenID.Top);
            Statics.Screens.OpenImmediate(ScreenID.Bottom);
            yield return Statics.Screens.TransitionToCO(ScreenID.Pillars);
        }

        public void LaunchActivity(ActivityEnum activity)
        {
            CurrentActivity = Statics.Data.Get<ActivityDefinition>((int)activity);
            SceneManager.LoadScene(CurrentActivity.sceneName, LoadSceneMode.Additive);
        }

        public void CloseActivity()
        {
            if (CurrentActivity)
            {
                SceneManager.UnloadSceneAsync(CurrentActivity.sceneName);
            }
            CurrentActivity = null;
        }
    }
}