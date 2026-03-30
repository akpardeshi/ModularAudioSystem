using System;
using UnityEngine;

namespace ModularAudio.Scripts
{
    [CreateAssetMenu(fileName = "AudioEventChannelSO", menuName = "Scriptable Objects/AudioEventChannelSO")]
    public class AudioEventChannelSo : ScriptableObject
    {
        private Action<AudioClip, float> _onEventRaised;

        public event Action<AudioClip, float> OnEventRaised
        {
            add => _onEventRaised += value;
            remove => _onEventRaised -= value;
        }

        public void RaiseEvent(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                Debug.LogWarning("[Audio] Play event raised with null clip.");
                return;
            }

            _onEventRaised?.Invoke(clip, volume);
        }
    }
}