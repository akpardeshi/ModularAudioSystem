using UnityEngine;

namespace ModularAudio.Scripts
{
    public class SfxAudioPlayer : AudioPlayerBase, IAudioControllable
    {
        public void PlayAudio(AudioClip clip, float volume = 1.0f)
        {
            AudioSource.PlayOneShot(clip, volume);
        }

        public void StopAudio()
        {
            Debug.LogWarning("[SfxAudioPlayer] StopAudio is not supported for SFX. Use MusicPlayer for controllable audio playback.");
        }
    }
}