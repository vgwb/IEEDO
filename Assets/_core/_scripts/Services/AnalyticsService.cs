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
        private bool AnalyticsEnabled => Statics.App.ApplicationConfig.AnalyticsEnabled;

        async void Awake()
        {
            if (!AnalyticsEnabled)
                return;

            var options = new InitializationOptions();
            if (Statics.App.ApplicationConfig.AnalyticsDevEnv)
            {
                options.SetEnvironmentName("dev");
            }
            await UnityServices.InitializeAsync(options);
            // Debug.Log("Analytics Enabled");
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

        public void Activity(string activityCode, string result)
        {
            if (!AnalyticsEnabled)
                return;

            var parameters = new Dictionary<string, object>()
            {
                { "myActivity", activityCode },
                { "myActivityResult", result },
            };

            Events.CustomData("myActivity", parameters);
#if UNITY_EDITOR
            Events.Flush();
#endif
            // Debug.Log("Analytics myActivity");
        }

        public void Card(string action, string category = "")
        {
            if (!AnalyticsEnabled)
                return;

            var parameters = new Dictionary<string, object>()
            {
                { "myCardAction", action },
                { "myCardCategory", category },
            };

            Events.CustomData("myCard", parameters);
#if UNITY_EDITOR
            Events.Flush();
#endif
        }

        public void Score(int score, string action)
        {
            if (!AnalyticsEnabled)
                return;

            var parameters = new Dictionary<string, object>()
            {
                { "myScore", score },
                { "myCardAction", action },
            };

            Events.CustomData("myScore", parameters);
#if UNITY_EDITOR
            Events.Flush();
#endif
        }

        public void App(string action)
        {
            if (!AnalyticsEnabled)
                return;

            var parameters = new Dictionary<string, object>()
            {
                { "myAction", action },
            };

            Events.CustomData("myApp", parameters);
#if UNITY_EDITOR
            Events.Flush();
#endif
        }

    }
}
