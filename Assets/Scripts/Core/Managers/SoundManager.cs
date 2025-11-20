using UnityEngine;
using UnityEngine.UI;
using System;

namespace Core.Managers
{
    public class SoundManager : MonoBehaviour
    {
        #region Singleton
        public static SoundManager instance { get; private set; }
        #endregion

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

        #region Unity Lifecycle Methods
        private void Awake()
        {
            InitializeSingleton();
            InitializeAudioSources();
            InitializeSliders();
        }
        #endregion

        #region Initialization Methods
        private void InitializeSingleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioSources()
        {
            if (soundSource == null) soundSource = gameObject.AddComponent<AudioSource>();
            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

            float savedMusicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
            float savedSoundVolume = PlayerPrefs.GetFloat("soundVolume", 0.5f);
            musicSource.volume = savedMusicVolume;
            soundSource.volume = savedSoundVolume;
        }

        private void InitializeSliders()
        {
            if (musicSlider != null)
            {
                float vol = PlayerPrefs.GetFloat("musicVolume", 0.5f);
                musicSlider.value = vol;
                musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            }

            if (soundSlider != null)
            {
                float vol = PlayerPrefs.GetFloat("soundVolume", 0.5f);
                soundSlider.value = vol;
                soundSlider.onValueChanged.AddListener(ChangeSoundVolume);
            }
        }
        #endregion

        #region Sound Playback Methods
        public void PlaySound(AudioClip _sound)
        {
            if (_sound != null)
                soundSource.PlayOneShot(_sound);
        }

        public void PlaySoundWithVolume(AudioClip _sound, float volume)
        {
            if (_sound != null)
                soundSource.PlayOneShot(_sound, volume);
        }
        #endregion

        #region Volume Control Methods
        public void ChangeSoundVolume(float _volume)
        {
            soundSource.volume = _volume;
            PlayerPrefs.SetFloat("soundVolume", _volume);
            PlayerPrefs.Save();

            if (soundSlider != null)
            {
                soundSlider.value = _volume;
            }
            
            OnSoundVolumeChanged?.Invoke(_volume);
        }

        public void ChangeMusicVolume(float _volume)
        {
            musicSource.volume = _volume;
            PlayerPrefs.SetFloat("musicVolume", _volume);
            PlayerPrefs.Save();

            if (musicSlider != null)
            {
                musicSlider.value = _volume;
            }

            OnMusicVolumeChanged?.Invoke(_volume);
        }
        #endregion
    }
}
