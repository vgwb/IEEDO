using UnityEngine;

namespace Ieedo
{
    public class ArtManager : MonoBehaviour
    {
        public PaletteColor UIColor;
        public float TitleSaturation;
        public float BGSaturation;

        public Color ToBG(Color col)
        {
            return col;
        }

        public Color ToTitle(Color col)
        {
            return col.SetSaturation(0.5f).SetValue(0.5f);
        }
    }
}
