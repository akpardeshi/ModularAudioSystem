using System.Collections.Generic;
using EventChannel.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace ModularAudio.Scripts
{
    public class AudioUIManager : MonoBehaviour
    {
        [Header("Audio Sliders"), SerializeField]
        private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        // event channels
        [Header("Volume Event Channels"), SerializeField]
        private FloatEventChannel masterVolume;
        [SerializeField] private FloatEventChannel musicVolume;
        [SerializeField] private FloatEventChannel sfxVolume;
        
        [Header("Audio Mixer Channel SO"), SerializeField]
        private List<AudioMixerChannelSo> audioChannels;
        
        // channel mapping
        private readonly Dictionary<AudioMixerNames, AudioMixerChannelSo> _channelMap = new();

        private const float InitialVolume = 0.7f;
        
        private void InitializeAudioChannels()
        {
            foreach (var channel in audioChannels)
            {
                if (channel == null)
                    continue;

                _channelMap.TryAdd(channel.channel, channel);
            }
        }

        private void OnEnable()
        {
            SubscribeToEvents(); 
        }

        private void Start()
        {
            InitializeAudioChannels();
            InitializeAudioVolume();
        }

        private void OnDisable()
        {
            UnsubscribeToEvents();
        }

        private void OnMasterVolume(float value)
        {
            masterVolume?.RaiseEvent(value);
        }
        
        private void OnMusicVolume(float value)
        {
            musicVolume?.RaiseEvent(value);
        }
        
        private void OnSfxVolume(float value)
        {
            sfxVolume?.RaiseEvent(value);
        }

        private void SubscribeToEvents()
        {
            if (masterVolumeSlider)
            {
                masterVolumeSlider.onValueChanged.RemoveAllListeners();
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolume);
            }
            
            else
            {
                Debug.LogWarning($"Please assign the master volume slider");
            }
            
            if (musicVolumeSlider)
            {
                musicVolumeSlider.onValueChanged.RemoveAllListeners();
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolume);
            }
            
            else
            {
                Debug.LogWarning($"Please assign the music volume slider");
            }
            
            if (sfxVolumeSlider)
            {
                sfxVolumeSlider.onValueChanged.RemoveAllListeners();
                sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolume);
            }
            
            else
            {
                Debug.LogWarning($"Please assign the sfx volume slider");
            }
        }
        
        private void UnsubscribeToEvents()
        {
            if (masterVolumeSlider)
            {
                masterVolumeSlider.onValueChanged.RemoveAllListeners();
            }

            else
            {
                Debug.LogWarning($"Please assign the master volume slider");
            }
            
            if (musicVolumeSlider)
            {
                musicVolumeSlider.onValueChanged.RemoveAllListeners();
            }

            else
            {
                Debug.LogWarning($"Please assign the music volume slider");
            }
            
            if (sfxVolumeSlider)
            {
                sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            }

            else
            {
                Debug.LogWarning($"Please assign the sfx volume slider");
            }
        }
        
        private void InitializeAudioVolume()
        {
            float volume = 0;
            _channelMap.TryGetValue(AudioMixerNames.MasterVolume, out var defaultMasterChannel);
            volume = defaultMasterChannel ? defaultMasterChannel.defaultVolume : InitialVolume;
            masterVolume?.RaiseEvent(volume);
            masterVolumeSlider?.SetValueWithoutNotify(volume);

            _channelMap.TryGetValue(AudioMixerNames.MusicVolume, out var defaultMusicChannel);
            volume = defaultMusicChannel ? defaultMusicChannel.defaultVolume : InitialVolume;
            musicVolume?.RaiseEvent(volume);
            musicVolumeSlider?.SetValueWithoutNotify(volume);
            
            _channelMap.TryGetValue(AudioMixerNames.SfxVolume, out var defaultSfxChannel);
            volume = defaultSfxChannel ? defaultSfxChannel.defaultVolume : InitialVolume;
            sfxVolume?.RaiseEvent(volume);
            sfxVolumeSlider?.SetValueWithoutNotify(volume);
        }
    }
}