using UnityEngine;

namespace ModularAds.Scripts.Core.Audio
{
    public class SfxAudioPlayer : AudioPlayerBase, IAudioControllable
    {
        public void PlayAudio(AudioClip clip)
        {
            AudioSource.PlayOneShot(clip);
        }

        public void StopAudio()
        {
            
        }
    }
}