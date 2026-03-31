using UnityEngine;

namespace ModularAudio.Scripts
{
    public struct AudioWrapper
    {
        public AudioClip clip;
        public float volume;

        public AudioWrapper(AudioClip clip, float volume)
        {
            this.clip = clip;
            this.volume = volume;
        }
    }
}