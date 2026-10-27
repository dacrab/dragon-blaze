using UnityEngine;

namespace Core.Managers
{
    /// <summary>Contract implemented by the persistent AudioManager.</summary>
    public interface IAudioManager
    {
        float MusicVolume { get; }
        float SoundVolume { get; }

        void PlayMusic(AudioClip? clip);
        void PlaySound(AudioClip? clip);
        void SetMusicVolume(float volume);
        void SetSoundVolume(float volume);
    }
}
