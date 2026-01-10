using UnityEngine;

namespace Core.Constants
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "DragonBlaze/Config/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Scenes")]
        public int mainMenuSceneIndex;
        public int firstLevelSceneIndex = 1;
        
        [Header("Audio")]
        [Range(0, 1)] public float defaultSoundVolume = 0.5f;
        [Range(0, 1)] public float defaultMusicVolume = 0.5f;
        
        [Header("Time")]
        public float pausedTimeScale;
        public float normalTimeScale = 1f;
        
        [Header("Save")]
        public string saveFileName = "savefile.json";
        public string musicVolumeKey = "musicVolume";
        public string soundVolumeKey = "soundVolume";
        
        [Header("UI")]
        public string coinDisplayFormat = ": {0}";
        public float navigationThreshold = 0.5f;
    }
}
