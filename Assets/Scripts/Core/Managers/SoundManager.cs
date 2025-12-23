using UnityEngine;
using UnityEngine.UI;
using System;
using Core.Constants;
using Core.Services;
using Core.Utilities;

namespace Core.Managers
{
    public class SoundManager : SingletonManager<SoundManager>
    {

        #region Events
        public event Action<float> OnSoundVolumeChanged;
        public event Action<float> OnMusicVolumeChanged;
        #endregion

        #region Serialized Fields
        [Header("Audio Sources")]
        [SerializeField] private AudioSource soundSource;
        [SerializeField] private AudioSource musicSource;

        [Header("UI References")]
        public Slider musicSlider;
        public Slider soundSlider;
        #endregion

        #region Initialization
        protected override void OnInitialize()
        {
            base.OnInitialize();
            InitializeAudioSources();
            InitializeSliders();
            ServiceLocator.Register<SoundManager>(this);
        }

        protected override void OnShutdown()
        {
            ServiceLocator.Unregister<SoundManager>();
            base.OnShutdown();
        }

        private void InitializeAudioSources()
        {
            if (soundSource == null) soundSource = gameObject.GetOrAddComponent<AudioSource>();
            if (musicSource == null) musicSource = gameObject.GetOrAddComponent<AudioSource>();

            float savedMusicVolume = PlayerPrefs.GetFloat(GameConstants.Save.MusicVolume, 0.5f);
            float savedSoundVolume = PlayerPrefs.GetFloat(GameConstants.Save.SoundVolume, 0.5f);
            musicSource.volume = savedMusicVolume;
            soundSource.volume = savedSoundVolume;
        }

        private void InitializeSliders()
        {
            if (musicSlider != null)
            {
                float vol = PlayerPrefs.GetFloat(GameConstants.Save.MusicVolume, 0.5f);
                musicSlider.value = vol;
                musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            }

            if (soundSlider != null)
            {
                float vol = PlayerPrefs.GetFloat(GameConstants.Save.SoundVolume, 0.5f);
                soundSlider.value = vol;
                soundSlider.onValueChanged.AddListener(ChangeSoundVolume);
            }
        }
        #endregion

        #region Sound Playback Methods
        public void PlaySound(AudioClip sound)
        {
            if (sound != null && soundSource != null)
                soundSource.PlayOneShot(sound);
        }

        public void PlaySoundWithVolume(AudioClip sound, float volume)
        {
            if (sound != null && soundSource != null)
                soundSource.PlayOneShot(sound, Mathf.Clamp01(volume));
        }

        public void PlayMusic(AudioClip music, bool loop = true)
        {
            if (music != null && musicSource != null)
            {
                musicSource.clip = music;
                musicSource.loop = loop;
                musicSource.Play();
            }
        }

        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
        #endregion

        #region Volume Control Methods
        public void ChangeSoundVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            soundSource.volume = volume;
            PlayerPrefs.SetFloat(GameConstants.Save.SoundVolume, volume);
            PlayerPrefs.Save();

            if (soundSlider != null)
            {
                soundSlider.value = volume;
            }
            
            OnSoundVolumeChanged?.Invoke(volume);
        }

        public void ChangeMusicVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            musicSource.volume = volume;
            PlayerPrefs.SetFloat(GameConstants.Save.MusicVolume, volume);
            PlayerPrefs.Save();

            if (musicSlider != null)
            {
                musicSlider.value = volume;
            }

            OnMusicVolumeChanged?.Invoke(volume);
        }
        #endregion
    }
}
