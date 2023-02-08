using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Ieedo
{
    public class ApplicationConfig : ScriptableObject
    {

#if UNITY_EDITOR
        public static ApplicationConfig FindMainConfig()
        {
            var configPath = $"Assets/_data/ApplicationConfig.asset";
            var config = AssetDatabase.LoadAssetAtPath<ApplicationConfig>(configPath);
            if (config == null)
            {
                Debug.LogError($"Could not find ApplicationConfig at path '{configPath}'");
                return null;
            }
            return config;
        }
#endif

        [Header("App")]
        /// <summary>
        /// Incremental version
        /// </summary>
        public int Version = 1;

        /// <summary>
        /// If true, the profile will be reset if the version of the profile does not match the version of the application.
        /// </summary>
        public bool ResetProfileAtVersionMismatch = true;

        public string UrlSupportWebsite;
        public int PointsCardCompleted;
        public int PointsCardValidated;

        public string PointsSymbol;
        public string GetPointsSymbolString()
        {
            return $"<color=#AD7C25>{System.Text.RegularExpressions.Regex.Unescape(PointsSymbol)}</color>";
        }

        [Header("Services")]
        public bool NotificationsEnabled;
        public bool AnalyticsEnabled;
        public bool AnalyticsDevEnv;

        [Header("Development")]
        public bool DebugMode;

        /// <summary>
        /// Switches on all Debug.Log calls for performance.
        /// Set to FALSE for production.
        /// </summary>
        public bool DebugLogEnabled = true;
        public bool DebugSaveProfileInJSON = true;
        public bool EnableSafeAreaInEditor;

        /// <summary>
        /// If true, cards generated in the editor will be saved to the default cards instead of the profile cards
        /// </summary>
        public bool SaveCardsAsDefault = false;

    }
}
