using UnityEngine;

namespace ModularAudio.Scripts
{
    public interface IAudioControllable
    {
        void PlayAudio(AudioClip clip, float volume);
        void StopAudio();
    }
}