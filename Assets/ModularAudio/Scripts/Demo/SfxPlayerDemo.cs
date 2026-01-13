using ModularAds.Core.Scripts.ScriptableEventChannels;
using ModularAds.Scripts.Core.Audio;
using UnityEngine;

namespace ModularAds.Scripts.Demo
{
    public class SfxPlayerDemo : MonoBehaviour,  IAudioPlayable
    {
        [SerializeField] private AudioClip[] clips;

        [field: SerializeField] public AudioEventChannelSo AudioEventChannelSo { get; private set; }

        public void PlayAudio(AudioClip audioClip)
        {
            if (!AudioEventChannelSo)
            {
                Debug.LogWarning("AudioEventChannelSo is not set", gameObject);
            }
            
            AudioEventChannelSo.RaiseEvent(audioClip);
        }

        public void PlayAudio1()
        {
            PlayAudio(clips[0]);
        }

        public void PlayAudio2()
        {
            PlayAudio(clips[1]);
        }

        public void PlayAudio3()
        {
            PlayAudio(clips[2]);
        }
    }
}