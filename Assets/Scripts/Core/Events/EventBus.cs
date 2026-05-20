using System;
using Core.Constants;
using UnityEngine;

namespace Core.Events
{
    public static class EventBus
    {
        public static event Action<int> OnScoreChanged;
        public static event Action OnPlayerDied;
        public static event Action OnPlayerRespawn;
        public static event Action<int> OnLevelLoaded;
        public static event Action<bool> OnGamePaused;
        public static event Action OnLevelCompleted;
        public static event Action<GameState> OnGameStateChanged;
        public static event Action<bool> OnDialogueStateChanged;
        public static event Action<float, float> OnHealthChanged;
        public static event Action<string, Sprite, float> OnPowerUpActivated;
        public static event Action OnRequestNextLevel;

        public static void RaiseScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
        public static void RaisePlayerRespawn() => OnPlayerRespawn?.Invoke();
        public static void RaiseLevelLoaded(int index) => OnLevelLoaded?.Invoke(index);
        public static void RaiseGamePaused(bool paused) => OnGamePaused?.Invoke(paused);
        public static void RaiseLevelCompleted() => OnLevelCompleted?.Invoke();
        public static void RaiseGameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);
        public static void RaiseDialogueStateChanged(bool open) => OnDialogueStateChanged?.Invoke(open);
        public static void RaiseHealthChanged(float current, float max) => OnHealthChanged?.Invoke(current, max);
        public static void RaisePowerUpActivated(string name, Sprite icon, float duration) => OnPowerUpActivated?.Invoke(name, icon, duration);
        public static void RaiseRequestNextLevel() => OnRequestNextLevel?.Invoke();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            OnScoreChanged = null;
            OnPlayerDied = null;
            OnPlayerRespawn = null;
            OnLevelLoaded = null;
            OnGamePaused = null;
            OnLevelCompleted = null;
            OnGameStateChanged = null;
            OnDialogueStateChanged = null;
            OnHealthChanged = null;
            OnPowerUpActivated = null;
            OnRequestNextLevel = null;
        }
    }
}
