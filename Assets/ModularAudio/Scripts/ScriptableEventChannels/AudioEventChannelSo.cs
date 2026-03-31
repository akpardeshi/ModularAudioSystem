using System;
using EventChannel.Scripts;
using UnityEngine;

namespace ModularAudio.Scripts
{
    [CreateAssetMenu(fileName = "AudioEventChannelSO", menuName = "Scriptable Objects/AudioEventChannelSO")]
    public class AudioEventChannelSo : ScriptableEventChannelBase<AudioWrapper>
    {
        private Action<AudioWrapper> _onEventRaised;

        public event Action<AudioWrapper> OnEventRaised
        {
            add => _onEventRaised += value;
            remove => _onEventRaised -= value;
        }

        public void RaiseEvent(AudioWrapper wrapper)
        {
            if (wrapper.clip == null)
            {
                Debug.LogWarning("[Audio] Play event raised with null clip.");
                return;
            }

            _onEventRaised?.Invoke(wrapper);
        }
    }
}