using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Ieedo
{
    public class AppVersionLabel : MonoBehaviour
    {
        public TextMeshProUGUI Label;

        void Start()
        {
            if (Statics.App.ApplicationConfig.DebugMode)
            {

                Label.text = "v 0." + Statics.App.ApplicationConfig.Version.ToString();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

    }
}
