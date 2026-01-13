using UnityEngine;

namespace ModularAds.Scripts.Core.Audio
{
    [CreateAssetMenu(fileName = "AudioMixerChannelSO", menuName = "Scriptable Objects/AudioMixerChannelSO")]
    public class AudioMixerChannelSo : ScriptableObject
    {
        [Header("Logical Identity")]
        public ModularAudioManager.AudioMixerNames channel;

        [Header("Mixer Binding")]
        [Tooltip("Exact exposed parameter name in AudioMixer")]
        public string volumeParameter;
        
        [Header("Defaults")]
        [Range(0f, 1f)]
        public float defaultVolume = 1f;
    }
}