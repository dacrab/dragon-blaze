using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Managers;

namespace Core.State
{
    /// <summary>
    /// Manages game state transitions and provides state querying capabilities.
    /// Uses Unity's built-in singleton pattern via SingletonManager.
    /// </summary>
    public class GameStateManager : SingletonManager<GameStateManager>
    {
        #region Properties
        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        public GameState PreviousState { get; private set; }
        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SubscribeToEvents();
        }

        protected override void OnShutdown()
        {
            UnsubscribeFromEvents();
            base.OnShutdown();
        }

        private void SubscribeToEvents()
        {
            EventBus.OnGamePaused += HandleGamePaused;
            EventBus.OnDialogueStateChanged += HandleDialogueStateChanged;
            EventBus.OnPlayerDied += HandlePlayerDied;
            EventBus.OnPlayerRespawn += HandlePlayerRespawn;
            EventBus.OnLevelLoaded += HandleLevelLoaded;
            EventBus.OnLevelCompleted += HandleLevelCompleted;
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.OnGamePaused -= HandleGamePaused;
            EventBus.OnDialogueStateChanged -= HandleDialogueStateChanged;
            EventBus.OnPlayerDied -= HandlePlayerDied;
            EventBus.OnPlayerRespawn -= HandlePlayerRespawn;
            EventBus.OnLevelLoaded -= HandleLevelLoaded;
            EventBus.OnLevelCompleted -= HandleLevelCompleted;
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            PreviousState = CurrentState;
            CurrentState = newState;
            ApplyStateEffects(newState);
            EventBus.RaiseGameStateChanged(CurrentState);
        }

        public void ReturnToPreviousState()
        {
            if (PreviousState != CurrentState) ChangeState(PreviousState);
        }

        public bool IsState(GameState state) => CurrentState == state;
        public bool IsAnyState(params GameState[] states)
        {
            foreach (var state in states)
                if (CurrentState == state) return true;
            return false;
        }

        public bool IsPlaying => CurrentState == GameState.Gameplay;
        public bool IsPaused => CurrentState == GameState.Paused;
        public bool IsInDialogue => CurrentState == GameState.Dialogue;
        public bool IsGameOver => CurrentState == GameState.GameOver;

        private void ApplyStateEffects(GameState state)
        {
            switch (state)
            {
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case GameState.Dialogue:
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case GameState.Loading:
                    Time.timeScale = 0f;
                    break;
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
            }
        }

        private void HandleGamePaused(bool isPaused) => ChangeState(isPaused ? GameState.Paused : GameState.Gameplay);
        private void HandleDialogueStateChanged(bool isOpen) => ChangeState(isOpen ? GameState.Dialogue : GameState.Gameplay);
        private void HandlePlayerDied() => ChangeState(GameState.GameOver);
        private void HandlePlayerRespawn() => ChangeState(GameState.Gameplay);
        private void HandleLevelLoaded(int levelIndex) => ChangeState(GameState.Gameplay);
        private void HandleLevelCompleted() => ChangeState(GameState.Loading);
    }
}

