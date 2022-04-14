using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ieedo;
using Ieedo.games;

namespace minigame.tictac
{
    /// <summary>
    /// Controls a graphical representation of the current turn state.
    /// </summary>
    public class TurnStateImageScript : MonoBehaviour
    {
        public Sprite xSprite, oSprite;
        private Image curImage;

        private void Awake()
        {
            curImage = GetComponent<Image>();
        }

        public void UpdateImage()
        {
            if (GameController.controller.CurPlayerSymbol == GameController.PlayerSymbol.X)
            {
                curImage.sprite = xSprite;
                curImage.color = GameController.controller.xColor;
            }
            else
            {
                curImage.sprite = oSprite;
                curImage.color = GameController.controller.oColor;
            }
        }
    }
}
