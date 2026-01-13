using ModularAds.Core.Scripts.ScriptableEventChannels;
using UnityEngine;

namespace ModularAds.Scripts.Core.Audio
{
    public interface IAudioControllable
    {
        // AudioSource AudioSource { get; }

        // void Initialize(AudioSource source);

        void PlayAudio(AudioClip clip);
        void StopAudio();
    }

    public interface IAudioPlayable
    {
        AudioEventChannelSo AudioEventChannelSo { get; }

        void PlayAudio(AudioClip clip);
    }
}