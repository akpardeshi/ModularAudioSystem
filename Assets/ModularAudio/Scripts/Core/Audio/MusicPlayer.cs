using UnityEngine;

namespace ModularAds.Scripts.Core.Audio
{
    public class MusicPlayer : AudioPlayerBase, IAudioControllable
    {
        public void PlayAudio(AudioClip clip)
        {
            if (clip == AudioSource.clip) return;

            StopAudio();
            
            AudioSource.clip = clip;
            AudioSource.Play();
        }

        public void StopAudio()
        {
            if (!AudioSource.isPlaying) return;

            AudioSource.Stop();
        }
    }
}