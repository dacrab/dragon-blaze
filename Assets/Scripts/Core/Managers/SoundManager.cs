using UnityEngine;
using System;
using Core.Constants;

namespace Core.Managers
{

public interface ISoundManager
{
    float SoundVolume { get; }
    float MusicVolume { get; }
    event Action<float> OnSoundVolumeChanged;
    event Action<float> OnMusicVolumeChanged;
    void PlaySound(AudioClip sound);
    void PlayMusic(AudioClip music, bool loop = true);
    void StopMusic();
    void SetSoundVolume(float volume);
    void SetMusicVolume(float volume);
}

public sealed class SoundManager : SingletonManager<SoundManager>, ISoundManager
{
    public event Action<float> OnSoundVolumeChanged, OnMusicVolumeChanged;

    [SerializeField] private AudioSource soundSource, musicSource;
    [SerializeField] private GameConfig gameConfig;

    public float SoundVolume => soundSource?.volume ?? gameConfig.defaultSoundVolume;
    public float MusicVolume => musicSource?.volume ?? gameConfig.defaultMusicVolume;

    protected override void OnInit()
    {
        if (gameConfig == null)
            gameConfig = Resources.Load<GameConfig>("GameConfig");
        
        if (gameConfig == null)
        {
            Debug.LogWarning("GameConfig not found. Creating default settings.");
            gameConfig = ScriptableObject.CreateInstance<GameConfig>();
        }
            
        soundSource ??= gameObject.AddComponent<AudioSource>();
        musicSource ??= gameObject.AddComponent<AudioSource>();
        
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
        PlayerPrefs.SetFloat(gameConfig.soundVolumeKey, volume);
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