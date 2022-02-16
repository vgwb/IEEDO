using UnityEngine;
using System.Collections;

namespace Ieedo.games
{
    public class Minigame : MonoBehaviour
    {
        public ActivityDefinition Activity;
        public System.Action<ActivityResult> OnActivityEnd;

        public void CloseMinigame(ActivityResult result)
        {
            OnActivityEnd?.Invoke(result);
        }

    }
}