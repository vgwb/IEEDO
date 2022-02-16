using UnityEngine;

namespace Ieedo
{
    public class UIIconsBar : MonoBehaviour
    {
        public UIText[] Icons;

        public void SetValue(int v)
        {
            for (int i = 0; i < v; i++) Icons[i].color = Color.yellow;
            for (int i = v; i < Icons.Length; i++) Icons[i].color = Color.gray;
        }
    }
}