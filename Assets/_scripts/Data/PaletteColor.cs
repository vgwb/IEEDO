using UnityEngine;
using UnityEngine.Serialization;

namespace Ieedo
{
    [CreateAssetMenu(fileName = "Color", menuName = "Ieedo/Palette Color")]
    public class PaletteColor : ScriptableObject
    {
        public Color LightColor;
        [FormerlySerializedAs("Color")] public Color BaseColor;
        [FormerlySerializedAs("DesaturatedColor")] public Color DarkColor;
    }
}
