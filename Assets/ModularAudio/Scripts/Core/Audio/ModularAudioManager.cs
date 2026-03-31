using System.Collections.Generic;
using EventChannel.Scripts;
using UnityEngine;
using UnityEngine.Audio;

namespace ModularAudio.Scripts
{
    public class ModularAudioManager : MonoBehaviour
    {
        #region Variables

        [Header("Event Channels"), SerializeField]
        private AudioEventChannelSo musicEventChannelSo;

        [SerializeField] private AudioEventChannelSo sfxEventChannelSo;

        [Header("Audio Sources"), SerializeField]
        private AudioSource sfxSource;

        [SerializeField] private AudioSource musicSource;

        [Header("Audio Mixer"), SerializeField]
        private AudioMixer audioMixerGroup;

        [Header("Audio Mixer Channel SO"), SerializeField]
        private List<AudioMixerChannelSo> audioChannels;

        // Audio Players 
        private MusicPlayer _musicPlayer;
        private SfxAudioPlayer _sfxPlayer;

        // Interfaces
        private IAudioControllable MusicPlayer => _musicPlayer;
        private IAudioControllable SfxPlayer => _sfxPlayer;

        // volume parameters
        private float _musicVolume;
        private bool _isMusicMuted;

        private float _sfxVolume;
        private bool _isSfxMuted;

        private float _masterVolume;
        private bool _isMasterMuted;

        // channel mapping
        private readonly Dictionary<AudioMixerNames, AudioMixerChannelSo> _channelMap = new();
        
        // event channels
        [Header("Volume Event Channels"), SerializeField]
        private FloatEventChannel masterVolumeEventChannel;
        [SerializeField] private FloatEventChannel musicVolumeEventChannel;
        [SerializeField] private FloatEventChannel sfxVolumeEventChannel;

        #endregion


        #region Event Functions
        
        void Awake()
        {
            InitializeAudioChannels();
            Initialize();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToEvents();
        }

        #endregion


        #region Event Handlers

        private void OnPlayMusicEventRaised(AudioWrapper wrapper)
        {
            MusicPlayer.PlayAudio(wrapper.clip, wrapper.volume);
        }

        private void OnPlaySfxEventRaised(AudioWrapper wrapper)
        {
            SfxPlayer.PlayAudio(wrapper.clip, wrapper.volume);
        }
        
        private void SubscribeToEvents()
        {
            if (masterVolumeEventChannel)
            {
                masterVolumeEventChannel.OnEventRaised += SetMasterVolume;
            }

            else
            {
                Debug.LogWarning($"[Audio] Master Volume is not assigned in the Inspector.");
            }
            
            if (musicVolumeEventChannel)
            {
                musicVolumeEventChannel.OnEventRaised += SetMusicVolume;
            }

            else
            {
                Debug.LogWarning($"[Audio] Music Volume is not assigned in the Inspector.");
            }
            
            if (sfxVolumeEventChannel)
            {
                sfxVolumeEventChannel.OnEventRaised += SetSfxVolume;
            }

            else
            {
                Debug.LogWarning($"[Audio] SFX Volume is not assigned in the Inspector.");
            }
            
            if (musicEventChannelSo)
            {
                musicEventChannelSo.OnEventRaised += OnPlayMusicEventRaised;
            }

            else
            {
                Debug.LogWarning($"[Audio] MusicEventChannelSo is not assigned in the Inspector.");
            }

            if (sfxEventChannelSo)
            {
                sfxEventChannelSo.OnEventRaised += OnPlaySfxEventRaised;
            }
            
            else
            {
                Debug.LogWarning($"[Audio] sfxEventChannelSo is not assigned in the Inspector.");
            }
        }
        
        private void UnsubscribeToEvents()
        {
            if (masterVolumeEventChannel)
            {
                masterVolumeEventChannel.OnEventRaised -= SetMasterVolume;
            }

            else
            {
                Debug.LogWarning($"[Audio] Master Volume is not assigned in the Inspector.");
            }
            
            if (musicVolumeEventChannel)
            {
                musicVolumeEventChannel.OnEventRaised -= SetMusicVolume;
            }

            else
            {
                Debug.LogWarning($"[Audio] Music Volume is not assigned in the Inspector.");
            }
            
            if (sfxVolumeEventChannel)
            {
                sfxVolumeEventChannel.OnEventRaised -= SetSfxVolume;
            }

            else
            {
                Debug.LogWarning($"[Audio] SFX Volume is not assigned in the Inspector.");
            }
            
            if (musicEventChannelSo)
            {
                musicEventChannelSo.OnEventRaised -= OnPlayMusicEventRaised;
            }

            else
            {
                Debug.LogWarning($"[Audio] MusicEventChannelSo is not assigned in the Inspector.");
            }

            if (sfxEventChannelSo)
            {
                sfxEventChannelSo.OnEventRaised -= OnPlaySfxEventRaised;
            }
            
            else
            {
                Debug.LogWarning($"[Audio] sfxEventChannelSo is not assigned in the Inspector.");
            }
        }

        #endregion


        #region Initialization

        private void Initialize()
        {
            if (!musicSource)
            {
                Debug.LogError($"[Audio] Music Source is not assigned in the Inspector.");
            }
            else
            {
                _musicPlayer = new MusicPlayer();
                _musicPlayer.Initialize(musicSource);
            }

            if (!sfxSource)
            {
                Debug.LogError($"[Audio] Sfx Source is not assigned in the Inspector.");
            }

            else
            {
                _sfxPlayer = new SfxAudioPlayer();
                _sfxPlayer.Initialize(sfxSource);
            }
        }

        private void InitializeAudioChannels()
        {
            foreach (var channel in audioChannels)
            {
                if (channel == null)
                    continue;

                _channelMap.TryAdd(channel.channel, channel);
            }
        }

        #endregion


        #region Audio Controls

        private void SetChannelVolume(AudioMixerNames channel, float normalizedVolume)
        {
            if (!_channelMap.TryGetValue(channel, out var channelSo))
            {
                Debug.LogError($"[Audio] Missing AudioMixerChannelSO for {channel}");
                return;
            }

            float db = LinearToDb(normalizedVolume);
            if (channelSo != null)
            {
                audioMixerGroup.SetFloat(channelSo.volumeParameter, db);
            }
        }

        private float LinearToDb(float linear)
        {
            if (linear <= 0.0001f)
                return -80f;

            return Mathf.Log10(linear) * 20f;
        }

        private void ApplyMusicVolume()
        {
            float effectiveVolume = _isMusicMuted ? 0f : _musicVolume;
            SetChannelVolume(AudioMixerNames.MusicVolume, effectiveVolume);
        }

        private void ApplyMasterVolume()
        {
            float effectiveVolume = _isMasterMuted ? 0f : _masterVolume;
            SetChannelVolume(AudioMixerNames.MasterVolume, effectiveVolume);
        }

        private void ApplySfxVolume()
        {
            float effectiveVolume = _isSfxMuted ? 0f : _sfxVolume;
            SetChannelVolume(AudioMixerNames.SfxVolume, effectiveVolume);
        }

        #endregion


        #region UI Functions

        private void SetMasterVolume(float sliderValue)
        {
            _masterVolume = sliderValue;
            ApplyMasterVolume();
        }

        private void SetMusicVolume(float sliderValue)
        {
            _musicVolume = sliderValue;
            ApplyMusicVolume();
        }

        private void SetSfxVolume(float sliderValue)
        {
            _sfxVolume = sliderValue;
            ApplySfxVolume();
        }

        public void ToggleMaster(bool value)
        {
            _isMasterMuted = value;
            ApplyMasterVolume();
        }

        public void ToggleSfx(bool value)
        {
            _isSfxMuted = value;
            ApplySfxVolume();
        }

        public void ToggleMusic(bool value)
        {
            _isMusicMuted = value;
            ApplyMusicVolume();
        }

        #endregion
    }
}