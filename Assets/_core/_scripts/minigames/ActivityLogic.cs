using UnityEngine;
using System.Collections;

namespace Ieedo.games
{
    public class ActivityLogic : MonoBehaviour
    {
        public ActivityDefinition Activity;
        public System.Action<ActivityResult> OnActivityEnd;

        public void CloseActivity(ActivityResult result)
        {
            OnActivityEnd?.Invoke(result);
        }

    }
}