using System;
using UnityEngine;
using NotificationSamples;

namespace Ieedo
{
    public class NotificationService : MonoBehaviour
    {
        private bool NotificationsEnabled => Statics.App.ApplicationConfig.NotificationsEnabled;

        public GameNotificationsManager NotificationsManager;

        public const string ChannelId = "game_channel0";
        private bool inizialized = false;
        private GameObject myGameObject;
        protected int playReminderHour = 18;

        // public NotificationService(GameObject _gameObject)
        // {
        //     myGameObject = _gameObject;
        //     Init();
        // }

        public void Init()
        {
            if (NotificationsEnabled)
            {
                if (!inizialized)
                {
                    Debug.Log("NotificationService Init");
                    //NotificationsManager = myGameObject.AddComponent<GameNotificationsManager>();

                    var channel = new GameNotificationChannel(ChannelId, "Default Game Channel", "Generic notifications");
                    NotificationsManager.Initialize(channel);
                    inizialized = true;
                }
            }
        }

        #region main
        /// <summary>
        /// automatically call everything to setup Notifications at AppSuspended
        /// </summary>
        public void AppSuspended()
        {
            if (NotificationsEnabled)
            {
                PrepareNextLocalNotification();
            }
        }

        /// <summary>
        /// automatically restore all Notifications at AppResumed
        /// </summary>
        public void AppResumed()
        {
            // if (!NotificationsManager.Initialized)
            // {
            //     Init();
            // }
            NotificationsManager.DismissAllNotifications();
        }
        #endregion

        private void PrepareNextLocalNotification()
        {
            //DeleteAllLocalNotifications();
            //Debug.Log("Next Local Notifications prepared");
            ScheduleNotification(
                GetDateTimeTomorrow(),
                "IEEDO App",
                LocString.FromStr("UI/notifications_remind_24h").GetLocalizedString()
            );

            //NotificationManager.ScheduleSimpleWithAppIcon(
            //    TimeSpan.FromSeconds(60),
            //    "Antura and the Letters",
            //    "Test notification after closing the app [60 seconds]",
            //    Color.blue
            //);
        }

        #region direct plugins methods

        public void ScheduleNotification(DateTime deliveryTime, string title, string message)
        {
            IGameNotification notification = NotificationsManager.CreateNotification();

            notification.Title = title;
            notification.Body = message;
            notification.Group = ChannelId;
            notification.DeliveryTime = deliveryTime;
            notification.LargeIcon = "icon_antura";
            NotificationsManager.ScheduleNotification(notification);

            Debug.Log("ScheduleNotification - " + deliveryTime);
        }

        public void DeleteAllLocalNotifications()
        {

        }
        #endregion

        #region time utilities
        private DateTime GetDateTimeTomorrow()
        {
            DateTime deliveryTime = DateTime.Now.ToLocalTime().AddDays(1);
            deliveryTime = new DateTime(deliveryTime.Year, deliveryTime.Month, deliveryTime.Day, playReminderHour, 0, 0,
                DateTimeKind.Local);
            return deliveryTime;
            //return DateTime.Now.AddHours(20);
        }

        private DateTime GetDateTimeInMinutes(int minutes)
        {
            return DateTime.Now.ToLocalTime().AddMinutes(minutes);
        }

        private int CalculateSecondsToTomorrowMidnight()
        {
            TimeSpan ts = DateTime.Today.AddDays(2).Subtract(DateTime.Now);
            return (int)ts.TotalSeconds;
        }
        #endregion

        #region tests
        public void TestLocalNotification()
        {
            //Debug.Log("Tomorrows midnight is in " + CalculateSecondsToTomorrowMidnight() + " seconds");
            //            var description = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Notification_24h).NativeText;
            var description = LocString.FromStr("UI/notifications_remind_24h").GetLocalizedString();
            ScheduleNotification(
                GetDateTimeInMinutes(1),
                "IEEDO App",
                description
            );
        }
        #endregion
    }
}
