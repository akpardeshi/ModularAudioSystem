using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        public IAudioControllable MusicPlayer => _musicPlayer;
        public IAudioControllable SfxPlayer => _sfxPlayer;

        // Initial Volume
        private const float InitialVolume = 0.7f;

        // volume parameters
        private float _musicVolume;
        private bool _isMusicMuted;

        private float _sfxVolume;
        private bool _isSfxMuted;

        private float _masterVolume;
        private bool _isMasterMuted;

        // channel mapping
        private readonly Dictionary<AudioMixerNames, AudioMixerChannelSo> _channelMap = new();

        [Header("Audio Sliders"), SerializeField]
        private Slider masterVolumeSlider;

        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        #endregion


        #region Event Functions

        private void OnEnable()
        {
            if (musicEventChannelSo)
            {
                musicEventChannelSo.OnEventRaised += OnPlayMusicEventRaised;
            }

            if (sfxEventChannelSo)
            {
                sfxEventChannelSo.OnEventRaised += OnPlaySfxEventRaised;
            }
        }

        void Awake()
        {
            InitializeAudioChannels();
            Initialize();
            SetVolume();

            SetChannelVolume(AudioMixerNames.MasterVolume, _masterVolume);
            SetChannelVolume(AudioMixerNames.MusicVolume, _musicVolume);
            SetChannelVolume(AudioMixerNames.SfxVolume, _sfxVolume);

            if (masterVolumeSlider)
            {
                masterVolumeSlider.SetValueWithoutNotify(_masterVolume);
            }
            else
            {
                Debug.LogWarning($"Please assign the master volume slider");
            }

            if (musicVolumeSlider)
            {
                musicVolumeSlider.SetValueWithoutNotify(_musicVolume);
            }
            else
            {
                Debug.LogWarning($"Please assign the music volume slider");
            }

            if (sfxVolumeSlider)
            {
                sfxVolumeSlider.SetValueWithoutNotify(_sfxVolume);
            }
            else
            {
                Debug.LogWarning($"Please assign the sfx volume slider");
            }
        }

        private void OnDisable()
        {
            if (musicEventChannelSo)
            {
                musicEventChannelSo.OnEventRaised -= OnPlayMusicEventRaised;
            }

            if (sfxEventChannelSo)
            {
                sfxEventChannelSo.OnEventRaised -= OnPlaySfxEventRaised;
            }
        }

        #endregion


        #region Event Handlers

        private void OnPlayMusicEventRaised(AudioClip audioClip, float volume)
        {
            _musicPlayer.PlayAudio(audioClip, volume);
        }

        private void OnPlaySfxEventRaised(AudioClip audioClip, float volume)
        {
            _sfxPlayer.PlayAudio(audioClip, volume);
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

        private void SetVolume()
        {
            _masterVolume = InitialVolume;
            _musicVolume = InitialVolume;
            _sfxVolume = InitialVolume;
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

        public void SetMasterVolume(float sliderValue)
        {
            _masterVolume = sliderValue;
            ApplyMasterVolume();
        }

        public void SetMusicVolume(float sliderValue)
        {
            _musicVolume = sliderValue;
            ApplyMusicVolume();
        }

        public void SetSfxVolume(float sliderValue)
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