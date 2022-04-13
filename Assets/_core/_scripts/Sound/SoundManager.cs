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
            audioSource = gameObject.GetComponent<AudioSource>();
        }

        public void PlaySfx(SfxEnum sfx)
        {
            Debug.Log("PlaySfx " + sfx);
            if (Statics.Data.Profile.Description.SfxDisabled)
                return;


            var sound = SoundsList.Sounds.Find(item => item.id == sfx);
            if (sound != null)
            {
                audioSource.clip = sound.audioClip;
                audioSource.volume = sound.Volume;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("PlaySfx does not exist sound " + sfx.ToString());
            }
        }
    }
}
