using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using Core.Constants;
using Core.Services;

namespace Core.Managers
{
    /// <summary>
    /// Built-in-driven audio: routes music/SFX through an optional AudioMixer with exposed
    /// volume parameters, falls back to per-source volumes when no mixer is wired, and pools
    /// one-shot SFX AudioSources. Register a mixer in a scene instance for full control.
    /// </summary>
    public sealed class AudioManager : MonoBehaviour, IAudioManager
    {
        const string MusicVolumeParam = "MusicVolume";
        const string SoundVolumeParam = "SoundVolume";

        [SerializeField] AudioMixer mixer;
        [SerializeField] AudioMixerGroup musicGroup, soundGroup;

        static AudioManager instance;

        AudioSource musicSource;
        ObjectPool<AudioSource> soundSources;

        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSoundVolumeChanged;

        public float MusicVolume { get; private set; }
        public float SoundVolume { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            if (instance != null) return;
            _ = new GameObject(nameof(AudioManager)).AddComponent<AudioManager>();
        }

        void Awake()
        {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<IAudioManager>(this);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            if (musicGroup != null) musicSource.outputAudioMixerGroup = musicGroup;

            var poolRoot = new GameObject("SoundSources");
            poolRoot.transform.SetParent(transform);
            soundSources = new ObjectPool<AudioSource>(
                createFunc: () =>
                {
                    var source = poolRoot.AddComponent<AudioSource>();
                    source.playOnAwake = false;
                    if (soundGroup != null) source.outputAudioMixerGroup = soundGroup;
                    return source;
                },
                actionOnGet: source => source.gameObject.SetActive(true),
                actionOnRelease: source => source.gameObject.SetActive(false));

            var config = GameConfig.Default;
            MusicVolume = PlayerPrefs.GetFloat(config.musicVolumeKey, config.defaultMusicVolume);
            SoundVolume = PlayerPrefs.GetFloat(config.soundVolumeKey, config.defaultSoundVolume);
            ApplyVolumes();
        }

        void OnDestroy()
        {
            if (instance != this) return;
            instance = null;
            ServiceLocator.Unregister<IAudioManager>();
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || musicSource == null) return;
            if (musicSource.isPlaying && musicSource.clip == clip) return;
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip == null || soundSources == null) return;
            var source = soundSources.Get();
            if (mixer == null) source.volume = SoundVolume;
            source.PlayOneShot(clip);
            _ = ReleaseWhenDoneAsync(source, clip.length);
        }

        async Awaitable ReleaseWhenDoneAsync(AudioSource source, float duration)
        {
            float endTime = Time.unscaledTime + duration;
            while (Time.unscaledTime < endTime)
                await Awaitable.NextFrameAsync();
            if (soundSources != null && source != null) soundSources.Release(source);
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(GameConfig.Default.musicVolumeKey, MusicVolume);
            ApplyVolumes();
            OnMusicVolumeChanged?.Invoke(MusicVolume);
        }

        public void SetSoundVolume(float volume)
        {
            SoundVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(GameConfig.Default.soundVolumeKey, SoundVolume);
            ApplyVolumes();
            OnSoundVolumeChanged?.Invoke(SoundVolume);
        }

        void ApplyVolumes()
        {
            if (mixer == null)
            {
                if (musicSource != null) musicSource.volume = MusicVolume;
                return;
            }
            mixer.SetFloat(MusicVolumeParam, ToDecibels(MusicVolume));
            mixer.SetFloat(SoundVolumeParam, ToDecibels(SoundVolume));
        }

        static float ToDecibels(float linear) => linear <= 0f ? -80f : Mathf.Log10(linear) * 20f;
    }
}
