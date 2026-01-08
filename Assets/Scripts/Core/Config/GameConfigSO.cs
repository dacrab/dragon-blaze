using UnityEngine;

namespace Core.Config;

[CreateAssetMenu(fileName = "GameConfig", menuName = "DragonBlaze/Config/Game Config")]
public sealed class GameConfigSO : ScriptableObject
{
    [Header("Scenes")]
    public int mainMenuSceneIndex;
    public int firstLevelSceneIndex = 1;

    [Header("Audio Defaults")]
    [Range(0, 1)] public float defaultMusicVolume = 0.5f;
    [Range(0, 1)] public float defaultSoundVolume = 0.5f;

    [Header("Save Keys")]
    public string saveFileName = "/savefile.json";
    public string musicVolumeKey = "musicVolume";
    public string soundVolumeKey = "soundVolume";

    [Header("Gameplay")]
    public float pausedTimeScale;
    public float normalTimeScale = 1f;

    [Header("Loading")]
    public float loadingCompleteThreshold = 0.9f;
    public float loadingActivationDelay = 0.3f;
}
