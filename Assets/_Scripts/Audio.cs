using System;
using UnityEngine;

namespace Gismo.Core.Audio
{
    [Serializable]
    public class Audio
    {
        public string name;

        public AudioClip clip;

        public string audioGroup;

        [Range(0f, 1f)]
        public float volume = 1f;

        [Range(.1f, 3f)]
        public float pitch = 1f;

        public bool isLoop;
        public bool hasCooldown;
        public AudioSource source;
    }
}
