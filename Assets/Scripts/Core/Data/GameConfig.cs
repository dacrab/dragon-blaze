using UnityEngine;

namespace Core.Data
{
    /// <summary>
    /// Central game configuration ScriptableObject for easy tweaking and testing.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "DragonBlaze/Config/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Game Settings")]
        [SerializeField] private float defaultTimeScale = 1f;
        [SerializeField] private bool enableDebugMode = false;
        
        [Header("Player Settings")]
        [SerializeField] private float defaultPlayerHealth = 100f;
        [SerializeField] private int defaultExtraJumps = 2;
        
        [Header("Audio Settings")]
        [SerializeField] [Range(0f, 1f)] private float defaultMasterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float defaultMusicVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float defaultSoundVolume = 0.8f;
        
        [Header("Performance")]
        [SerializeField] private int maxPoolSize = 50;
        [SerializeField] private bool enableObjectPooling = true;

        // Properties
        public float DefaultTimeScale => defaultTimeScale;
        public bool EnableDebugMode => enableDebugMode;
        public float DefaultPlayerHealth => defaultPlayerHealth;
        public int DefaultExtraJumps => defaultExtraJumps;
        public float DefaultMasterVolume => defaultMasterVolume;
        public float DefaultMusicVolume => defaultMusicVolume;
        public float DefaultSoundVolume => defaultSoundVolume;
        public int MaxPoolSize => maxPoolSize;
        public bool EnableObjectPooling => enableObjectPooling;
    }
}

