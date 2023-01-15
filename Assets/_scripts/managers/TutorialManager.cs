using UnityEngine;

namespace Ieedo
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject TutorialArrow;

        private GameObject targetGo;

        public void ShowTutorialArrowOn(GameObject go, float orientation = 0f)
        {
            TutorialArrow.SetActive(true);
            targetGo = go;
            TutorialArrow.transform.position = go.transform.position;
            TutorialArrow.transform.localEulerAngles = new Vector3(0, 0, orientation);
        }

        public void Update()
        {
            if (TutorialArrow != null && TutorialArrow.activeSelf)
            {
                TutorialArrow.transform.position = targetGo.transform.position;
            }
        }

        public void HideTutorialArrow()
        {
            TutorialArrow.SetActive(false);
        }
    }
}
