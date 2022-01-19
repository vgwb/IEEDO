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
            {var _ = Statics.Flow;}

            // Load the game
            // yield return FlowManager.TransitionTo(SceneID.Intro);
            yield break;
        }
    }
}