using System.Collections.Generic;
using ModularAds.Core.Scripts.ScriptableEventChannels;
using UnityEngine;
using UnityEngine.Audio;

namespace ModularAds.Scripts.Core.Audio
{
    public class ModularAudioManager : MonoBehaviour
    {
        #region Enums

        public enum AudioMixerNames
        {
            MasterVolume = 0,
            MusicVolume = 1,
            SfxVolume = 2
        }

        #endregion


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

        void Start()
        {
            InitializeAudioChannels();
            Initialize();

            SetChannelVolume(AudioMixerNames.MasterVolume, InitialVolume);
            SetChannelVolume(AudioMixerNames.MusicVolume, InitialVolume);
            SetChannelVolume(AudioMixerNames.SfxVolume, InitialVolume);
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

        private void OnPlayMusicEventRaised(AudioClip audioClip)
        {
            _musicPlayer.PlayAudio(audioClip);
        }

        private void OnPlaySfxEventRaised(AudioClip audioClip)
        {
            _sfxPlayer.PlayAudio(audioClip);
        }

        #endregion


        #region Initialization

        private void Initialize()
        {
            if (!musicSource)
            {
                Debug.LogError($"[Audio] MusicSource missing: {musicSource != null}");
            }
            else
            {
                _musicPlayer = new MusicPlayer();
                _musicPlayer.Initialize(musicSource);
            }

            if (!sfxSource)
            {
                Debug.LogError($"[Audio] SfxSource missing: {sfxSource != null}");
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