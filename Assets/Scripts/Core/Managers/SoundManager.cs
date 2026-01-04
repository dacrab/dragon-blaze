using UnityEngine;
using UnityEngine.UI;
using System;
using Core.Constants;

namespace Core.Managers
{
    public class SoundManager : SingletonManager<SoundManager>
    {
        public event Action<float> OnSoundVolumeChanged;
        public event Action<float> OnMusicVolumeChanged;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource soundSource;
        [SerializeField] private AudioSource musicSource;

        [Header("UI References")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider soundSlider;

        protected override void OnInitialize()
        {
            InitializeAudioSources();
            InitializeSliders();
        }

        private void InitializeAudioSources()
        {
            if (soundSource == null) soundSource = gameObject.AddComponent<AudioSource>();
            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

            musicSource.volume = PlayerPrefs.GetFloat(GameConstants.Save.MusicVolume, 0.5f);
            soundSource.volume = PlayerPrefs.GetFloat(GameConstants.Save.SoundVolume, 0.5f);
        }

        private void InitializeSliders()
        {
            if (musicSlider != null)
            {
                musicSlider.value = PlayerPrefs.GetFloat(GameConstants.Save.MusicVolume, 0.5f);
                musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            }
            if (soundSlider != null)
            {
                soundSlider.value = PlayerPrefs.GetFloat(GameConstants.Save.SoundVolume, 0.5f);
                soundSlider.onValueChanged.AddListener(ChangeSoundVolume);
            }
        }

        public void PlaySound(AudioClip sound)
        {
            if (sound != null && soundSource != null)
                soundSource.PlayOneShot(sound);
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

        public void StopMusic() => musicSource?.Stop();

        public void ChangeSoundVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            soundSource.volume = volume;
            PlayerPrefs.SetFloat(GameConstants.Save.SoundVolume, volume);
            if (soundSlider != null) soundSlider.value = volume;
            OnSoundVolumeChanged?.Invoke(volume);
        }

        public void ChangeMusicVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            musicSource.volume = volume;
            PlayerPrefs.SetFloat(GameConstants.Save.MusicVolume, volume);
            if (musicSlider != null) musicSlider.value = volume;
            OnMusicVolumeChanged?.Invoke(volume);
        }
    }
}
