using UnityEngine;

namespace Core.Constants
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "DragonBlaze/Config/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        public const string DefaultResourcesPath = "GameConfig";

        static GameConfig? cached;

        /// <summary>Singleton config asset loaded from Resources. Never null in play mode.</summary>
        public static GameConfig Default
        {
            get
            {
                if (cached != null) return cached;
                cached = Resources.Load<GameConfig>(DefaultResourcesPath);
                if (cached == null)
                {
                    Debug.LogWarning($"[GameConfig] No '{DefaultResourcesPath}' asset in Resources; using code defaults.");
                    cached = ScriptableObject.CreateInstance<GameConfig>()!;
                }
                return cached;
            }
        }

        [Tooltip("Scene names in play order. Level order drives next-level navigation; entry 0 is the main menu.")]
        public string[] levelOrder =
        {
            "MainMenu", "Level1", "Level2", "Level3", "Level4", "Credits"
        };

        [Tooltip("Scenes that behave like menus (cursor free, UI input) rather than playable levels. Menu scenes and any credits/outro screens go here.")]
        public string[] menuScenes = { "MainMenu", "Credits" };

        [Header("Audio")]
        [Range(0, 1)] public float defaultSoundVolume = 0.5f;
        [Range(0, 1)] public float defaultMusicVolume = 0.5f;
        [Tooltip("Track played in the main menu scene; levels play levelMusic.")]
        public AudioClip? menuMusic;
        [Tooltip("Track played in every scene except the main menu.")]
        public AudioClip? levelMusic;

        [Header("UI")]
        public float navigationThreshold = 0.5f;

        [Header("Save")]
        public string saveFileName = "savefile.json";
        public string musicVolumeKey = "musicVolume";
        public string soundVolumeKey = "soundVolume";

        [Header("State")]
        [Tooltip("Optional per-state overrides. Any state missing here falls back to DefaultStateSettings.")]
        public StateSettings[]? stateSettings;

        public string? MainMenuSceneName => levelOrder.Length > 0 ? levelOrder[0] : null;
        public string? FirstLevelSceneName => levelOrder.Length > 1 ? levelOrder[1] : null;

        /// <summary>True for menu-type scenes (see <see cref="menuScenes"/>): free cursor, UI input, no gameplay state.</summary>
        public bool IsMenuScene(string? sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return false;
            if (menuScenes != null)
                foreach (var name in menuScenes)
                    if (name == sceneName) return true;
            return false;
        }

        public bool TryGetNextLevel(string current, out string? next)
        {
            int index = System.Array.IndexOf(levelOrder, current);
            if (index >= 0 && index + 1 < levelOrder.Length)
            {
                next = levelOrder[index + 1];
                return true;
            }
            next = null;
            return false;
        }

        public StateSettings GetStateSettings(GameState state)
        {
            if (stateSettings != null)
                foreach (var settings in stateSettings)
                    if (settings != null && settings.state == state) return settings;
            return DefaultStateSettings(state);
        }

        public static StateSettings DefaultStateSettings(GameState state) => state switch
        {
            GameState.Gameplay or GameState.Loading =>
                new StateSettings { state = state, timeScale = 1f, cursorVisible = false, cursorLocked = true },
            GameState.MainMenu =>
                new StateSettings { state = state, timeScale = 1f, cursorVisible = true, cursorLocked = false },
            _ =>
                new StateSettings { state = state, timeScale = 0f, cursorVisible = true, cursorLocked = false },
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetCache() => cached = null;
    }

    [System.Serializable]
    public sealed class StateSettings
    {
        public GameState state;
        public float timeScale = 1f;
        public bool cursorVisible = true;
        public bool cursorLocked;
    }
}
