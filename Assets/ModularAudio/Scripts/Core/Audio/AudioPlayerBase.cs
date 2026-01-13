using UnityEngine;

namespace ModularAds.Scripts.Core.Audio
{
    public abstract class AudioPlayerBase 
    {
        protected AudioSource AudioSource;

        public void Initialize(AudioSource source)
        {
            AudioSource = source;
        }
    }
}