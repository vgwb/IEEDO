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

                Label.text = "v " + (Statics.App.ApplicationConfig.Version / 100).ToString("N2");
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

    }
}
