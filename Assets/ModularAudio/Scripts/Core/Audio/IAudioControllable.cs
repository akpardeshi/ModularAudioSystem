using UnityEngine;

namespace ModularAudio.Scripts
{
    public interface IAudioControllable
    {
        void PlayAudio(AudioClip clip);
        void StopAudio();
    }
}