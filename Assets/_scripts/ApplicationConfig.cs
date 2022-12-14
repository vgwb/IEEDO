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

        [Header("Debug")]
        /// <summary>
        /// Switches on all Debug.Log calls for performance.
        /// Set to FALSE for production.
        /// </summary>
        public bool DebugLogEnabled = true;

        public bool DebugSaveProfileInJSON = true;

        [Header("App")]
        /// <summary>
        /// Incremental version
        /// </summary>
        public int Version = 1;

        /// <summary>
        /// If true, the profile will be reset if the version of the profile does not match the version of the application.
        /// </summary>
        public bool ResetProfileAtVersionMismatch = true;

        /// <summary>
        /// If true, cards generated in the editor will be saved to the default cards instead of the profile cards
        /// </summary>
        public bool SaveCardsAsDefault = false;

        [Header("Notifications")]
        public bool NotificationsEnabled;

        [Header("Analytics")]
        public bool AnalyticsEnabled;
        public bool AnalyticsDevEnv;
    }
}
