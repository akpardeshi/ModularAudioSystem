using UnityEngine;

namespace ModularAudio.Scripts
{
    public class SfxAudioPlayer : AudioPlayerBase, IAudioControllable
    {
        public void PlayAudio(AudioClip clip)
        {
            AudioSource.PlayOneShot(clip);
        }

        public void StopAudio()
        {
            AudioSource.Stop();
        }
    }
}