using UnityEngine;

namespace ModularAudio.Scripts
{
    [CreateAssetMenu(fileName = "AudioMixerChannelSO", menuName = "Scriptable Objects/AudioMixerChannelSO")]
    public class AudioMixerChannelSo : ScriptableObject
    {
        [Header("Logical Identity")]
        public AudioMixerNames channel;

        [Header("Mixer Binding")]
        [Tooltip("Exact exposed parameter name in AudioMixer")]
        public string volumeParameter;
    }
}