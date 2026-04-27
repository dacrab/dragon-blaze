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
    }
}
