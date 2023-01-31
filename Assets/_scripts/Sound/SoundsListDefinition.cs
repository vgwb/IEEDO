using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ieedo
{

    public enum SfxEnum
    {
        ui_click = 1,
        game_win = 2,
        game_lose = 3,
        open = 4,
        close = 5,
        trash = 6,
        score = 7,
        changeScreen = 8,
        card_positive = 9,
        card_negative = 10
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

        [Range(0.0f, 1.0f)]
        public float Volume = 1.0f;
    }
}
