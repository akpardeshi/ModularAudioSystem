# Modular Audio System

A modular, event-driven audio system for Unity that manages music and SFX playback through ScriptableObject event channels — fully decoupled from game systems, configurable from the Inspector, and extensible without modifying existing code.

> **Unity version:** 6000.3.8f1 · **Language:** C# · **Status:** Phase 1 — portfolio showcase

---

## The problem it solves

Audio in Unity projects typically ends up tightly coupled — `AudioManager.Instance.PlaySound()` calls scattered across gameplay code, volume sliders directly referencing the audio manager, and no clean way to swap or silence audio per-system without hunting down every reference.

This system solves it with three design principles. First, any system raises an audio event by firing a ScriptableObject channel — it never calls the audio manager directly. Second, volume control is handled through a separate `FloatEventChannel` pipeline, keeping UI completely decoupled from the audio manager. Third, per-channel mixer configuration lives in ScriptableObject assets, so volume defaults and mixer bindings are set by designers in the Inspector without touching code.

---

## Architecture

The system has four distinct layers that communicate only through ScriptableObject event channels.

```
┌─────────────────────────────────────────────────────────┐
│                    CONSUMER LAYER                        │
│   Any MonoBehaviour raises AudioEventChannelSo           │
│   MusicPlayerDemo / SfxPlayerDemo (demo scripts)        │
└────────────────────────┬────────────────────────────────┘
                         │ RaiseEvent(AudioWrapper)
                         ▼
┌─────────────────────────────────────────────────────────┐
│               AUDIO EVENT CHANNELS                       │
│   AudioEventChannelSo — ScriptableObject asset          │
│   Action<AudioWrapper> — encapsulated event             │
└────────────────────────┬────────────────────────────────┘
                         │ OnEventRaised
                         ▼
┌─────────────────────────────────────────────────────────┐
│               MODULAR AUDIO MANAGER                      │
│   Subscribes to audio and volume event channels         │
│   Manages MusicPlayer and SfxAudioPlayer instances      │
│   LinearToDb conversion · mute state tracking           │
└──────────┬─────────────────────────────┬────────────────┘
           │                             │
           ▼                             ▼
┌──────────────────┐         ┌──────────────────────────┐
│   MusicPlayer    │         │     SfxAudioPlayer        │
│  AudioPlayerBase │         │    AudioPlayerBase        │
│  PlayAudio(...)  │         │  PlayOneShot(...)         │
│  StopAudio()     │         │  StopAudio — unsupported  │
└──────────────────┘         └──────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                  UI LAYER (DECOUPLED)                    │
│   AudioUIManager — owns all Slider references           │
│   Raises FloatEventChannel on slider value change       │
│   Initialises slider values from AudioMixerChannelSo    │
└────────────────────────┬────────────────────────────────┘
                         │ RaiseEvent(float volume)
                         ▼
┌─────────────────────────────────────────────────────────┐
│               FLOAT EVENT CHANNELS                       │
│   FloatEventChannel — one per volume type               │
│   masterVolume · musicVolume · sfxVolume                │
└────────────────────────┬────────────────────────────────┘
                         │ OnEventRaised → SetMasterVolume etc.
                         ▼
                ModularAudioManager
                         │
                         ▼
                    AudioMixer
```

### Key design decisions

**ScriptableObject event channels for audio** — any game system that needs to play a sound raises an `AudioEventChannelSo` asset using an `AudioWrapper` payload. The audio manager listens on that channel. Neither side has a direct reference to the other. Adding or removing systems never requires touching the audio manager.

**AudioWrapper payload** — audio playback data is wrapped in a lightweight `AudioWrapper` struct that contains the clip and volume. This keeps the event signature stable while allowing future extensibility without breaking subscribers.

**Separate UI pipeline via FloatEventChannel** — volume sliders communicate through a `FloatEventChannel`, not direct method calls on the manager. `AudioUIManager` and `ModularAudioManager` are completely unaware of each other. Both only know about the shared channel asset. This means the UI can be replaced, removed, or tested independently.

**AudioPlayerBase + interface injection** — `MusicPlayer` and `SfxAudioPlayer` are plain C# classes, not MonoBehaviours. They receive their `AudioSource` via `Initialize(source)` rather than `GetComponent`. This makes them independently testable and removes the per-frame overhead of MonoBehaviour lifecycle methods.

**LinearToDb conversion** — volume is stored and exposed as normalised 0–1 values throughout the system, converted to decibels only at the AudioMixer boundary using `Mathf.Log10(linear) * 20f`. This gives UI controls a natural linear feel while the mixer receives the correct logarithmic scale.

**Per-channel default volume in ScriptableObject** — each `AudioMixerChannelSo` asset defines its own `defaultVolume`. The system reads these on initialisation, ensuring volume defaults are data-driven and configurable without code changes.

---

## Setup

### 1. Create ScriptableObject assets

Right-click in the Project window to create the required assets:

**Audio event channels** (one per audio type you need):
`Scriptable Objects > AudioEventChannelSO`

**Mixer channel configuration** (one per AudioMixer group):
`Scriptable Objects > AudioMixerChannelSO`

Set the `Volume Parameter` field on each to match the exact exposed parameter name in your AudioMixer asset. Set `Default Volume` to your desired starting value (0–1).

**Float event channels** (one per volume control):
`Systems > ScriptableEventChannels > FloatEventChannel`

Create three: `MasterVolumeChannel`, `MusicVolumeChannel`, `SfxVolumeChannel`.

### 2. Scene setup

Create a GameObject and attach `ModularAudioManager`. Assign:

* Music and SFX `AudioSource` components
* Your `AudioMixer` asset
* The three `AudioMixerChannelSo` assets
* The music and SFX `AudioEventChannelSo` assets
* The three `FloatEventChannel` assets

Create a second GameObject and attach `AudioUIManager`. Assign:

* The three UI `Slider` references
* The same three `FloatEventChannel` assets
* The same three `AudioMixerChannelSo` assets (for default volume initialisation)

### 3. Play audio from any script

```csharp
[SerializeField] private AudioEventChannelSo sfxChannel;

sfxChannel.RaiseEvent(new AudioWrapper
{
    clip = audioClip,
    volume = 1f
});
```

The audio manager receives the wrapper payload and routes playback to the appropriate player.

### 4. Mute controls

The mute toggles are public and can be wired directly to UI Toggle components:

```csharp
// Wire these to UnityEvent callbacks on Toggle components in the Inspector
audioManager.ToggleMaster(bool value);
audioManager.ToggleMusic(bool value);
audioManager.ToggleSfx(bool value);
```

---

## How the volume pipeline works

```
User moves slider
      │
      ▼
AudioUIManager.OnMasterVolume(float value)
      │
      ▼
masterVolumeChannel.RaiseEvent(value)       ← FloatEventChannel SO
      │
      ▼
ModularAudioManager.SetMasterVolume(value)  ← subscribed in OnEnable
      │
      ▼
_masterVolume = value
ApplyMasterVolume()                         ← respects mute state
      │
      ▼
SetChannelVolume(AudioMixerNames.MasterVolume, effectiveVolume)
      │
      ▼
LinearToDb(normalizedVolume)
      │
      ▼
audioMixerGroup.SetFloat(volumeParameter, db)
```

Neither `AudioUIManager` nor `ModularAudioManager` holds a reference to the other. Both are replaceable independently.

---

## SFX behaviour — known limitation

`SfxAudioPlayer` uses `AudioSource.PlayOneShot(clip, volume)` which correctly allows multiple overlapping sounds. As a consequence, `StopAudio()` is not supported for SFX — stopping the shared `AudioSource` would cancel all currently playing one-shots simultaneously. A warning is logged if `StopAudio` is called on the SFX player.

---

## Known limitations and roadmap

**Phase 1 — current scope**

This is the foundational audio architecture. It handles playback, volume control, and mixer management through a fully decoupled event channel pipeline.

What is not yet included:

* **Crossfade** — `MusicPlayer.PlayAudio` stops the current clip and starts the new one immediately. Hard cuts between tracks will produce a click artifact. A coroutine-based crossfade with configurable duration is planned for Phase 2
* **Audio pooling** — SFX uses a single shared `AudioSource`. High-frequency or complex overlapping SFX scenarios will benefit from a pooled multi-source architecture in Phase 2
* **Volume persistence** — settings reset to `defaultVolume` from the ScriptableObject on every session start. `PlayerPrefs` save/load is planned for Phase 2
* **3D spatial audio** — all playback is currently 2D. Positional audio support via world-space `AudioSource` injection is a Phase 2 consideration

**Phase 2 roadmap**

* Coroutine-based music crossfade with configurable duration
* AudioSource pool for SFX with individual stop support
* PlayerPrefs volume persistence with save/load on session start/end
* UPM package preparation

---

## Dependencies

**Core system** — no external dependencies beyond Unity 6000.3.8f1 standard packages.

---

## Attribution

**Music**

* *Travelling* by Aylex — [https://freetouse.com/music](https://freetouse.com/music) — Copyright Free Music (Free Download)
* *Adventures Begin* by Pufino — [https://freetouse.com/music](https://freetouse.com/music) — No Copyright Music for Videos (Free)

**SFX**

* RPG Audio pack by Kenney — [https://kenney.nl/assets/rpg-audio](https://kenney.nl/assets/rpg-audio) — Creative Commons CC0

---

## Why I built this

Audio in Unity projects tends to accumulate coupling quietly. By the time a project is mid-size, `AudioManager.Instance` is called from fifty places, volume sliders have direct references to the manager, and changing anything requires touching code across the entire project.

The design goal here was a system where no game script needs to know the audio manager exists. Any script raises a channel event. The manager responds. The UI communicates through its own channel pipeline. Every layer is replaceable without the others knowing.

Building it surfaced a specific challenge — keeping the UI volume state synchronised with the mixer state without either layer holding a reference to the other. The `FloatEventChannel` pipeline solves this: `AudioUIManager` initialises slider values from `AudioMixerChannelSo` assets directly, then publishes all future changes through the channel. The audio manager only ever sees float values arriving on a channel — it never knows where they came from.

---

## Video Showcase

Watch the system in action:

**YouTube Demo:** [https://www.youtube.com/watch?v=7f0pq6SObw4](https://www.youtube.com/watch?v=7f0pq6SObw4)

---

## AudioWrapper payload

`AudioEventChannelSo` uses an `AudioWrapper` payload instead of passing raw parameters. This keeps the event signature stable while allowing additional playback data to be added without breaking subscribers.

Example structure:

```csharp
[System.Serializable]
public struct AudioWrapper
{
    public AudioClip clip;
    public float volume;
}
```

The wrapper is passed through the event channel:

```csharp
sfxChannel.RaiseEvent(new AudioWrapper
{
    clip = audioClip,
    volume = 1f
});
```

This design allows extending playback data later without changing the event signature. For example:

* loop flag
* pitch
* fade duration
* spatial settings
* mixer routing

---
