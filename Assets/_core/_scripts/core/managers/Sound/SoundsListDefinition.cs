using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ieedo
{

    public enum SfxEnum
    {
        click,
        win,
        lose
    }

    [CreateAssetMenu(menuName = "Ieedo/Sounds List")]
    public class SoundsListDefinition : ScriptableObject
    {
        public List<SoundItem> Sounds;
    }

    [Serializable]
    public class SoundItem
    {
        public SfxEnum id;
        public AudioClip audioClip;
    }
}
