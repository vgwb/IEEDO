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
        async void Awake()
        {
            var options = new InitializationOptions();
            options.SetEnvironmentName("dev");
            await UnityServices.InitializeAsync();
        }

    }
}
