using UnityEngine;

namespace Ieedo
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject TutorialArrow;

        public void ShowTutorialArrowOn(GameObject go, float orientation = 0f)
        {
            TutorialArrow.SetActive(true);
            TutorialArrow.transform.position = go.transform.position;
            TutorialArrow.transform.localEulerAngles = new Vector3(0, 0, orientation);
        }

        public void HideTutorialArrow()
        {
            TutorialArrow.SetActive(false);
        }
    }
}
