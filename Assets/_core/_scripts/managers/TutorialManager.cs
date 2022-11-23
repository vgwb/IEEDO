using UnityEngine;

namespace Ieedo
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject TutorialArrow;

        public void ShowTutorialArrowOn(GameObject go)
        {
            TutorialArrow.SetActive(true);
            TutorialArrow.transform.position = go.transform.position;
        }

        public void HideTutorialArrow()
        {
            TutorialArrow.SetActive(false);
        }
    }
}
