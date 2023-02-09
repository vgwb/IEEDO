using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ieedo
{
    public enum AudioEnum
    {
        ui_click = 1,
        points = 7,
        game_score = 11,
        game_win = 2,
        game_lose = 3,
        game_error = 12,
        game_bonus = 13,
        panel_open = 4,
        panel_close = 5,
        changeScreen = 8,
        card_trash = 6,
        card_positive = 9,
        card_negative = 10,
        music_1 = 100
    }

    public enum AudioChannel
    {
        Sfx1,
        Sfx2,
        Sfx3,
        Music
    }

    [CreateAssetMenu(menuName = "Ieedo/Sounds List")]
    public class SoundsListDefinition : ScriptableObject
    {
        public List<SoundItem> Sounds;
    }

    [Serializable]
    public class SoundItem
    {
        public AudioEnum id;
        public AudioClip audioClip;
        public AudioChannel Channel;

        [Range(0.0f, 1.0f)]
        public float Volume = 1.0f;
    }
}
