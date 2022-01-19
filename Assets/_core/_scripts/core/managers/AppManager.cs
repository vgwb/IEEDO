using System;
using System.Collections;
using System.Collections.Generic;
using Ieedo.Utilities;
using UnityEngine;

namespace Ieedo
{
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        public IEnumerator Start()
        {
            // Init data
            {var _ = Statics.Data;}
            {var _ = Statics.Cards;}
            {var _ = Statics.Screens;}

            Statics.Screens.LoadScreens();

            // Load the game
            Statics.Screens.OpenImmediate(ScreenID.Top);
            yield return Statics.Screens.TransitionToCO(ScreenID.Intro);
        }
    }
}