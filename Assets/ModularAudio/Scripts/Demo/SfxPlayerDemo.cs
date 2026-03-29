using ModularAudio.Scripts;
using UnityEngine;

namespace ModularAds.Scripts.Demo
{
    public class SfxPlayerDemo : MonoBehaviour, IAudioPlayable
    {
        [SerializeField] private AudioClip[] clips;

        [field: SerializeField] public AudioEventChannelSo AudioEventChannelSo { get; private set; }

        public void PlayAudio(AudioClip audioClip)
        {
            if (!AudioEventChannelSo)
            {
                Debug.LogWarning("AudioEventChannelSo is not set", gameObject);
                return;
            }

            AudioEventChannelSo.RaiseEvent(audioClip);
        }

        public void PlayAudio1()
        {
            PlayAudioAtIndex(0);
        }

        public void PlayAudio2()
        {
            PlayAudioAtIndex(1);
        }

        public void PlayAudio3()
        {
            PlayAudioAtIndex(2);
        }

        private void PlayAudioAtIndex(int index)
        {
            if (clips == null || index < 0 || index >= clips.Length)
            {
                Debug.LogError($"The audio clip with Index {index} does not exist");
                return;
            }

            var clip = clips[index];
            if (clip)
            {
                PlayAudio(clip);
            }
        }
    }
}