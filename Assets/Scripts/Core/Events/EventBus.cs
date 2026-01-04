using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;

namespace Core.Events
{
    public static class EventBus
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize() => ClearAll();

        // Player Events
        public static event Action<int> OnScoreChanged;
        public static event Action OnPlayerDied;
        public static event Action OnPlayerRespawn;

        // Game State Events
        public static event Action<int> OnLevelLoaded;
        public static event Action<bool> OnGamePaused;
        public static event Action OnGameSaved;
        public static event Action OnGameStarted;
        public static event Action OnLevelCompleted;
        public static event Action<GameState> OnGameStateChanged;

        // Dialogue Events
        public static event Action<bool> OnDialogueStateChanged;

        // Health Events
        public static event Action<float, float> OnHealthChanged;



        // Raise Methods
        public static void RaiseScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
        public static void RaisePlayerRespawn() => OnPlayerRespawn?.Invoke();
        public static void RaiseLevelLoaded(int level) => OnLevelLoaded?.Invoke(level);
        public static void RaiseGamePaused(bool paused) => OnGamePaused?.Invoke(paused);
        public static void RaiseGameSaved() => OnGameSaved?.Invoke();
        public static void RaiseGameStarted() => OnGameStarted?.Invoke();
        public static void RaiseLevelCompleted() => OnLevelCompleted?.Invoke();
        public static void RaiseGameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);
        public static void RaiseDialogueStateChanged(bool open) => OnDialogueStateChanged?.Invoke(open);
        public static void RaiseHealthChanged(float current, float max) => OnHealthChanged?.Invoke(current, max);

        public static void ClearAll()
        {
            OnScoreChanged = null;
            OnPlayerDied = null;
            OnPlayerRespawn = null;
            OnLevelLoaded = null;
            OnGamePaused = null;
            OnGameSaved = null;
            OnGameStarted = null;
            OnLevelCompleted = null;
            OnGameStateChanged = null;
            OnDialogueStateChanged = null;
            OnHealthChanged = null;
        }
    }
}
