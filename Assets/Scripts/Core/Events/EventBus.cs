using System;
using UnityEngine;

namespace Core.Events
{
    public static class EventBus
    {
        // Player Events
        public static event Action<int> OnScoreChanged;
        public static event Action OnPlayerDied;
        public static event Action OnPlayerRespawn;
        
        // Game State Events
        public static event Action<int> OnLevelLoaded;
        public static event Action<bool> OnGamePaused;
        public static event Action OnGameSaved;

        // Invocation Methods
        public static void RaiseScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
        public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
        public static void RaisePlayerRespawn() => OnPlayerRespawn?.Invoke();
        public static void RaiseLevelLoaded(int levelIndex) => OnLevelLoaded?.Invoke(levelIndex);
        public static void RaiseGamePaused(bool isPaused) => OnGamePaused?.Invoke(isPaused);
        public static void RaiseGameSaved() => OnGameSaved?.Invoke();
    }
}
