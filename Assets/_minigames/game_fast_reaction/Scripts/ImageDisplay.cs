using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace minigame.fast_reaction
{

    public class ImageDisplay : MonoBehaviour
    {
        public GameObject Image_1, Image_2;
        public Sprite[] Album;

        public void ShowImage(int which)
        {
            Image_1.GetComponent<Image>().sprite = Album[which];
        }

        public int GetAlbumSize()
        {
            return Album.Length;
        }

    }
}
