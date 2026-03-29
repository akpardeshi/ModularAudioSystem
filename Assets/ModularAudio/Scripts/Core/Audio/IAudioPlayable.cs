using UnityEngine;

namespace ModularAudio.Scripts
{
    public interface IAudioPlayable
    {
        AudioEventChannelSo AudioEventChannelSo { get; }

        void PlayAudio(AudioClip clip);
    }
}