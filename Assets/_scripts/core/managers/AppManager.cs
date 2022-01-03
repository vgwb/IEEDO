using System;
using System.Collections;
using System.Collections.Generic;
using Ieedo.Utilities;

namespace Ieedo
{
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        public IEnumerator Start()
        {
            // Init data
            {var _ = Statics.Data;}
            {var _ = Statics.Cards;}
            yield break;
        }
    }
}