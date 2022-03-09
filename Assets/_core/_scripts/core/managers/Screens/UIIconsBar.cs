using UnityEngine;

namespace Ieedo
{
    public class UIIconsBar : MonoBehaviour
    {
        public UIText[] Icons;

        public void SetValue(int v)
        {
            for (int i = 0; i < v; i++) Icons[i].color = Color.white;
            for (int i = v; i < Icons.Length; i++) Icons[i].color = new Color(1f, 1f, 1f, 0.3f);
        }
    }
}
