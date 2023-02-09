using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Transition;

namespace minigame.fast_reaction
{

    public class ImageDisplay : MonoBehaviour
    {
        public GameObject Image_1, Image_Left, Image_Right;
        public Sprite[] Album;

        public float Speed = 0.3f;

        private int newImage;
        private Vector3 startingPos;

        void Start()
        {
            startingPos = Image_1.transform.localPosition;
        }

        public int GetAlbumSize()
        {
            return Album.Length;
        }

        public void ShowImage(int which)
        {
            newImage = which;
            Image_1.GetComponent<Image>().sprite = Album[newImage];
        }

        public void NewImage(int which)
        {
            newImage = which;
            Image_1.transform.localPositionTransition(Image_Right.transform.localPosition, Speed, LeanEase.Accelerate);
            Image_1.transform.EventTransition(() => changeImage(), Speed + 0.05f);

        }

        private void changeImage()
        {
            Image_1.GetComponent<Image>().sprite = Album[newImage];
            Image_1.transform.localPosition = Image_Left.transform.localPosition;
            Image_1.transform.localPositionTransition(startingPos, Speed, LeanEase.Decelerate);
        }

    }
}
