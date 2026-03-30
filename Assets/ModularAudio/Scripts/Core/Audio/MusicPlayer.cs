using UnityEngine;

namespace ModularAudio.Scripts
{
    public class MusicPlayer : AudioPlayerBase, IAudioControllable
    {
        public void PlayAudio(AudioClip clip, float volume = 1.0f)
        {
            if (!AudioSource)
            {
                Debug.LogWarning($"There is no audio source assigned to music player.");
                return;
            }

            if (clip == AudioSource.clip)
            {
                Debug.LogWarning($"There is no audio clip  assigned to music player.");
                return;
            }

            StopAudio();
            
            AudioSource.volume = volume;
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