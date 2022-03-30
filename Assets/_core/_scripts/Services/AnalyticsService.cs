using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Analytics;

namespace Ieedo
{
    public class AnalyticsService : MonoBehaviour
    {
        private bool AnalyticsEnabled => Statics.App.ApplicationConfig.AnalyticsDevEnvironment;

        async void Awake()
        {
            if (!AnalyticsEnabled)
                return;

            var options = new InitializationOptions();

            if (Statics.App.ApplicationConfig.AnalyticsDevEnvironment)
            {
                options.SetEnvironmentName("dev");
            }
            await UnityServices.InitializeAsync();
        }

        public void TestEvent()
        {
            if (!AnalyticsEnabled)
                return;

            var parameters = new Dictionary<string, object>()
            {
                { "myNativeLang", Statics.App.ApplicationConfig.SourceLocale },
            };

            Events.CustomData("myTestEvent", parameters);
            Events.Flush();
            Debug.Log("Analytics TestEvent");
        }

        public void Activity(string result)
        {
            if (!AnalyticsEnabled)
                return;

            var parameters = new Dictionary<string, object>()
            {
                { "myActivityResult", result },
            };

            Events.CustomData("myActivity", parameters);
            if (Statics.App.ApplicationConfig.AnalyticsDevEnvironment)
                Events.Flush();
        }

        public void Card(string action)
        {
            if (!AnalyticsEnabled)
                return;

            var parameters = new Dictionary<string, object>()
            {
                { "myCardAction", action },
            };

            Events.CustomData("myCard", parameters);
            if (Statics.App.ApplicationConfig.AnalyticsDevEnvironment)
                Events.Flush();
        }
    }
}
