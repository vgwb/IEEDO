using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo
{

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager I;
        public SoundsListDefinition SoundsList;

        private AudioSource audioSource;

        public void Awake()
        {
            if (I == null)
                I = this;
            audioSource = this.gameObject.GetComponent<AudioSource>();
        }

        public void PlaySfx(SfxEnum sfx)
        {
            audioSource.clip = SoundsList.Sounds.Find(item => item.id == sfx).audioClip;
            audioSource.Play();
        }
    }
}
