using UnityEngine;
using System;
using Core.Constants;

namespace Core.Managers
{
    public sealed class SoundManager : SingletonManager<SoundManager>
    {
        public event Action<float> OnSoundVolumeChanged, OnMusicVolumeChanged;

        [SerializeField] AudioSource soundSource, musicSource;
        [SerializeField] GameConfig gameConfig;

        public float SoundVolume => soundSource?.volume ?? gameConfig.defaultSoundVolume;
        public float MusicVolume => musicSource?.volume ?? gameConfig.defaultMusicVolume;

        protected override void OnInit()
        {
            if (gameConfig == null) gameConfig = Resources.Load<GameConfig>("GameConfig");
            if (soundSource == null || musicSource == null)
            {
                Debug.LogError("SoundManager requires AudioSource components assigned in inspector");
                return;
            }
                
            musicSource.volume = PlayerPrefs.GetFloat(gameConfig.musicVolumeKey, gameConfig.defaultMusicVolume);
            soundSource.volume = PlayerPrefs.GetFloat(gameConfig.soundVolumeKey, gameConfig.defaultSoundVolume);
        }

        public void PlaySound(AudioClip sound) 
        { 
            if (sound != null) soundSource.PlayOneShot(sound); 
        }

        public void PlayMusic(AudioClip music, bool loop = true)
        {
            if (music == null) return;
            musicSource.clip = music;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void StopMusic() => musicSource.Stop();

        public void SetSoundVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            soundSource.volume = volume;
            PlayerPrefs.SetFloat(gameConfig.musicVolumeKey, volume);
            OnSoundVolumeChanged?.Invoke(volume);
        }

        public void SetMusicVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            musicSource.volume = volume;
            PlayerPrefs.SetFloat(gameConfig.musicVolumeKey, volume);
            OnMusicVolumeChanged?.Invoke(volume);
        }
    }
}
}