using System;
using UnityEngine;

namespace Core.Managers
{
    /// <summary>Contract implemented by the persistent AudioManager.</summary>
    public interface IAudioManager
    {
        float MusicVolume { get; }
        float SoundVolume { get; }
        event Action<float> OnMusicVolumeChanged;
        event Action<float> OnSoundVolumeChanged;

        void PlayMusic(AudioClip clip);
        void PlaySound(AudioClip clip);
        void SetMusicVolume(float volume);
        void SetSoundVolume(float volume);
    }
}
