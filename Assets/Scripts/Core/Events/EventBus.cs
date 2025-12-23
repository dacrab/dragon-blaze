using System;
using UnityEngine;
using Core.Constants;

namespace Core.Events
{
    /// <summary>
    /// Central event bus for decoupled communication between systems.
    /// Consider migrating to ScriptableObject-based GameEvent system for better editor integration.
    /// </summary>
    public static class EventBus
    {
        #region Player Events
        public static event Action<int> OnScoreChanged;
        public static event Action OnPlayerDied;
        public static event Action OnPlayerRespawn;
        public static event Action<float> OnPlayerHealthChanged;
        #endregion

        #region Game State Events
        public static event Action<int> OnLevelLoaded;
        public static event Action<bool> OnGamePaused;
        public static event Action OnGameSaved;
        public static event Action OnGameStarted;
        public static event Action OnLevelCompleted;
        public static event Action<GameState> OnGameStateChanged;
        #endregion

        #region Dialogue Events
        public static event Action<bool> OnDialogueStateChanged; // true = start, false = end
        #endregion

        #region Health Events
        public static event Action<float, float> OnHealthChanged; // current, max
        #endregion

        #region Combat Events
        public static event Action<float, DamageType> OnDamageDealt;
        public static event Action OnEnemyKilled;
        #endregion

        #region Invocation Methods - Player
        public static void RaiseScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
        public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
        public static void RaisePlayerRespawn() => OnPlayerRespawn?.Invoke();
        public static void RaisePlayerHealthChanged(float currentHealth) => OnPlayerHealthChanged?.Invoke(currentHealth);
        #endregion

        #region Invocation Methods - Game State
        public static void RaiseLevelLoaded(int levelIndex) => OnLevelLoaded?.Invoke(levelIndex);
        public static void RaiseGamePaused(bool isPaused) => OnGamePaused?.Invoke(isPaused);
        public static void RaiseGameSaved() => OnGameSaved?.Invoke();
        public static void RaiseGameStarted() => OnGameStarted?.Invoke();
        public static void RaiseLevelCompleted() => OnLevelCompleted?.Invoke();
        public static void RaiseGameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);
        #endregion

        #region Invocation Methods - Dialogue
        public static void RaiseDialogueStateChanged(bool isOpen) => OnDialogueStateChanged?.Invoke(isOpen);
        #endregion

        #region Invocation Methods - Health
        public static void RaiseHealthChanged(float current, float max) => OnHealthChanged?.Invoke(current, max);
        #endregion

        #region Invocation Methods - Combat
        public static void RaiseDamageDealt(float damage, DamageType damageType) => OnDamageDealt?.Invoke(damage, damageType);
        public static void RaiseEnemyKilled() => OnEnemyKilled?.Invoke();
        #endregion

        #region Cleanup
        /// <summary>
        /// Clears all event subscriptions. Use with caution, typically only for scene transitions.
        /// </summary>
        public static void ClearAll()
        {
            OnScoreChanged = null;
            OnPlayerDied = null;
            OnPlayerRespawn = null;
            OnPlayerHealthChanged = null;
            OnLevelLoaded = null;
            OnGamePaused = null;
            OnGameSaved = null;
            OnGameStarted = null;
            OnLevelCompleted = null;
            OnGameStateChanged = null;
            OnDialogueStateChanged = null;
            OnHealthChanged = null;
            OnDamageDealt = null;
            OnEnemyKilled = null;
        }
        #endregion
    }
}
