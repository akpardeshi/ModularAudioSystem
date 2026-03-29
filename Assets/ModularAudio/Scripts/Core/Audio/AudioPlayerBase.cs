using System;
using UnityEngine;

namespace ModularAudio.Scripts
{
    public abstract class AudioPlayerBase
    {
        protected AudioSource AudioSource;

        public void Initialize(AudioSource source)
        {
            if (!AudioSource)
            {
                Debug.LogError("[AudioPlayerBase] AudioSource cannot be null.");
                return;
            }

            AudioSource = source;
        }
    }
}