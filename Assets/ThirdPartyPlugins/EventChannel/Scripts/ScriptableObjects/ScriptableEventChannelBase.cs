using System;
using UnityEngine;

namespace EventChannel.Scripts
{
    public abstract class ScriptableEventChannelBase<T> : ScriptableObject
    {
        private Action<T> _onEventRaised;

        public event Action<T> OnEventRaised
        {
            add    => _onEventRaised += value;
            remove => _onEventRaised -= value;
        }

        public virtual void RaiseEvent(T eventData)
        {
            _onEventRaised?.Invoke(eventData);
        }
    }
}