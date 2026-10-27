using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Services;

namespace Core.Managers
{
    /// <summary>
    /// Persistent audio: music playback follows the active scene (menu track in the menu, level
    /// track otherwise) and one-shot SFX come from a pooled source. Volumes persist via PlayerPrefs.
    /// </summary>
    public sealed class AudioManager : MonoBehaviour, IAudioManager
    {
        static AudioManager? instance;

        AudioSource? musicSource;
        ObjectPool<AudioSource>? soundSources;
        AudioClip? currentMusic;

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

            var poolRoot = new GameObject("SoundSources");
            poolRoot.transform.SetParent(transform);
            soundSources = new ObjectPool<AudioSource>(
                createFunc: () =>
                {
                    var source = new GameObject("OneShotSource").AddComponent<AudioSource>();
                    source.transform.SetParent(poolRoot.transform);
                    source.playOnAwake = false;
                    return source;
                },
                actionOnGet: source => source.gameObject.SetActive(true),
                actionOnRelease: source => source.gameObject.SetActive(false),
                actionOnDestroy: source => Destroy(source.gameObject));

            var config = GameConfig.Default;
            MusicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(config.musicVolumeKey, config.defaultMusicVolume));
            SoundVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(config.soundVolumeKey, config.defaultSoundVolume));
            SceneManager.sceneLoaded += OnSceneLoaded;
            PlaySceneMusic(SceneManager.GetActiveScene().name);
        }

        void OnDestroy()
        {
            if (instance != this) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
            ServiceLocator.Unregister<IAudioManager>();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single) PlaySceneMusic(scene.name);
        }

        void PlaySceneMusic(string sceneName)
        {
            var config = GameConfig.Default;
            PlayMusic(sceneName == config.MainMenuSceneName ? config.menuMusic : config.levelMusic);
        }

        public void PlayMusic(AudioClip? clip)
        {
            if (musicSource == null || clip == currentMusic) return;
            currentMusic = clip;
            if (clip == null) { musicSource.Stop(); return; }
            musicSource.clip = clip;
            musicSource.volume = MusicVolume;
            musicSource.Play();
        }

        public void PlaySound(AudioClip? clip)
        {
            if (clip == null || soundSources == null) return;
            var source = soundSources.Get();

            // Play rather than PlayOneShot: one clip per pooled source, so a reused source can
            // never have a second clip layered on top of one that is still ringing.
            source.clip = clip;
            source.volume = SoundVolume;
            source.Play();
            _ = ReleaseWhenDoneAsync(source);
        }

        /// <summary>Returns the source to the pool once playback really finishes, on unscaled time so it returns while paused.</summary>
        async Awaitable ReleaseWhenDoneAsync(AudioSource source)
        {
            // isPlaying goes false when the clip ends; NextFrameAsync polls every frame, so this
            // also terminates if the source is deactivated underneath us.
            while (source != null && source.isPlaying)
                await Awaitable.NextFrameAsync();
            if (soundSources != null && source != null)
            {
                source.clip = null;
                soundSources.Release(source);
            }
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            if (musicSource != null) musicSource.volume = MusicVolume;
            SaveVolumes();
        }

        public void SetSoundVolume(float volume)
        {
            SoundVolume = Mathf.Clamp01(volume);
            SaveVolumes();
        }

        // PlayerPrefs only writes to disk on Save(); without it the sliders reset on every quit.
        void SaveVolumes()
        {
            var config = GameConfig.Default;
            PlayerPrefs.SetFloat(config.musicVolumeKey, MusicVolume);
            PlayerPrefs.SetFloat(config.soundVolumeKey, SoundVolume);
            PlayerPrefs.Save();
        }
    }
}
