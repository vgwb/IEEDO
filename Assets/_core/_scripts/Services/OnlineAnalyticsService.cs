using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Analytics;

namespace Ieedo
{
    public class OnlineAnalyticsService : MonoBehaviour
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
                Debug.LogWarning("Analytics in DEV environment");
            }
            await UnityServices.InitializeAsync(options);
        }

        public void TestEvent()
        {
            if (!AnalyticsEnabled)
                return;

            var parameters = new Dictionary<string, object>()
            {
                { "myNativeLang", Statics.App.ApplicationConfig.SourceLocale },
            };

            AnalyticsService.Instance.CustomData("myTestEvent", parameters);
            AnalyticsService.Instance.Flush();
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

            AnalyticsService.Instance.CustomData("myActivity", parameters);
#if UNITY_EDITOR
            AnalyticsService.Instance.Flush();
#endif
            // Debug.Log("Analytics myActivity");
        }

        public void Card(string action, CardData card)
        {
            if (!AnalyticsEnabled)
                return;

            var parameters = new Dictionary<string, object>()
            {
                { "myCardAction", action },
                { "myCardCategory", card.Definition.ToString() },
            };

            AnalyticsService.Instance.CustomData("myCard", parameters);
#if UNITY_EDITOR
            AnalyticsService.Instance.Flush();
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

            AnalyticsService.Instance.CustomData("myScore", parameters);
#if UNITY_EDITOR
            AnalyticsService.Instance.Flush();
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

            AnalyticsService.Instance.CustomData("myApp", parameters);
#if UNITY_EDITOR
            AnalyticsService.Instance.Flush();
#endif
        }

    }
}
