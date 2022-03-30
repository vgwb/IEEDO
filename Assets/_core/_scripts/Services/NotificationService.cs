using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vgwb.notifications;

namespace Ieedo
{
    public class NotificationService : MonoBehaviour
    {

        public void PrepareNotification()
        {

        }

        public void TestLocalNotification()
        {
            Debug.Log("TestLocalNotification");
            //Debug.Log("Tomorrows midnight is in " + CalculateSecondsToTomorrowMidnight() + " seconds");
            // var arabicString = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Notification_24h);
            // ScheduleSimple(
            //     GetDateTimeInMinues(1),
            //     "Antura and the Letters",
            //     arabicString.NativeText
            // );
        }
    }
}
