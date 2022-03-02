using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo.games
{
    public enum SfxEnum
    {
        click,
        win,
        lose
    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager I;

        public AudioClip Click;
        public AudioClip Win;
        public AudioClip Lose;

        private AudioSource audioSource;

        public void Awake()
        {
            if (I == null)
                I = this;
            audioSource = this.gameObject.GetComponent<AudioSource>();
        }

        public void PlaySfx(SfxEnum sfx)
        {
            switch (sfx)
            {
                case SfxEnum.click:
                    audioSource.clip = Click;
                    break;
                case SfxEnum.win:
                    audioSource.clip = Win;
                    break;
                case SfxEnum.lose:
                    audioSource.clip = Lose;
                    break;
                default:
                    break;
            }

            audioSource.Play();
        }
    }
}
