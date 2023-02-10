using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Transition;

namespace minigame.fast_reaction
{

    public class ImageDisplay : MonoBehaviour
    {
        public GameObject Image_main, Image_Left, Image_Right;
        public Sprite[] Album;

        public float Speed = 0.3f;

        private int newImage;
        private Vector3 startingPos;

        void Start()
        {
            startingPos = Image_main.transform.localPosition;
        }

        public int GetAlbumSize()
        {
            return Album.Length;
        }

        public void ShowImage(int which)
        {
            newImage = which;
            Image_main.GetComponent<Image>().sprite = Album[newImage];
        }

        public void NewImage(int which)
        {
            newImage = which;
            Image_main.transform.localPositionTransition(Image_Right.transform.localPosition, Speed, LeanEase.Accelerate);
            Image_main.transform.EventTransition(() => changeImage(), Speed + 0.05f);
        }

        private void changeImage()
        {
            Image_main.GetComponent<Image>().sprite = Album[newImage];
            Image_main.transform.localPosition = Image_Left.transform.localPosition;
            Image_main.transform.localPositionTransition(startingPos, Speed, LeanEase.Decelerate);
        }

    }
}
