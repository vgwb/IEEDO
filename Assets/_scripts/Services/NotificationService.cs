using System;
using System.Collections;
using UnityEngine;
using NotificationSamples;

namespace Ieedo
{
    public class NotificationService : MonoBehaviour
    {
        private bool NotificationsEnabled => Statics.App.ApplicationConfig.NotificationsEnabled;

        public GameNotificationsManager NotificationsManager;
        public const string ReminderChannelId = "reminder_channel1";
        private readonly string smallIconName = "icon_0";
        private readonly string largeIconName = "icon_1";
        protected int playReminderHour = 7;

        private IEnumerator Start()
        {
            if (NotificationsEnabled)
            {
                //                Debug.Log("NotificationService Init");
                var channel = new GameNotificationChannel(ReminderChannelId, "Reminder Channel", "Reminder notifications");
                yield return NotificationsManager.Initialize(channel);
                ScheduleRemindNotification();
            }
            else
            {
                yield return null;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                if (NotificationsEnabled)
                {
                }
            }
        }

        private void ScheduleRemindNotification()
        {
            // var remindMessage = LocString.FromStr("UI/notifications_remind_24h").GetLocalizedString();
            var remindMessage = "Remember to play and to update your to-do-list everyday!";

            ScheduleNotification(
                GetDateTimeTomorrow(),
                "IEEDO App",
                remindMessage
            );
            //            Debug.Log("Next Remind Notification prepared");
        }

        public void ScheduleNotification(DateTime deliveryTime, string title, string message)
        {
            IGameNotification notification = NotificationsManager.CreateNotification();
            if (notification == null)
            {
                return;
            }
            notification.Title = title;
            notification.Body = message;
            notification.Group = ReminderChannelId;
            notification.DeliveryTime = deliveryTime;
            notification.SmallIcon = smallIconName;
            notification.LargeIcon = largeIconName;

            PendingNotification notificationToDisplay = NotificationsManager.ScheduleNotification(notification);
            notificationToDisplay.Reschedule = false;
            Debug.Log("ScheduleNotification - " + deliveryTime);
        }

        public void DeleteAllLocalNotifications()
        {
            NotificationsManager.Platform.CancelAllScheduledNotifications();

        }

        #region time utilities
        private DateTime GetDateTimeTomorrow()
        {
            DateTime deliveryTime = DateTime.Now.ToLocalTime().AddDays(1);
            //DateTime deliveryTime = DateTime.Now.ToLocalTime().AddMinutes(2);
            deliveryTime = new DateTime(deliveryTime.Year, deliveryTime.Month, deliveryTime.Day, playReminderHour, 0, 0,
                DateTimeKind.Local);
            return deliveryTime;
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
            Debug.Log("Tomorrows midnight is in " + CalculateSecondsToTomorrowMidnight() + " seconds");
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
