using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Events;
using Core.Managers;

namespace Core.State
{
    /// <summary>
    /// Manages game state transitions and provides state querying capabilities.
    /// Integrates with Unity's SceneManager for automatic state handling.
    /// </summary>
    public class GameStateManager : SingletonManager<GameStateManager>
    {
        #region Properties
        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        public GameState PreviousState { get; private set; }
        public bool IsTransitioning { get; private set; }
        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SubscribeToEvents();
            SubscribeToSceneEvents();
            
            // Set initial state based on current scene
            DetermineInitialState();
        }

        protected override void OnShutdown()
        {
            UnsubscribeFromEvents();
            UnsubscribeFromSceneEvents();
            base.OnShutdown();
        }

        #region Scene Event Integration
        private void SubscribeToSceneEvents()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void UnsubscribeFromSceneEvents()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            IsTransitioning = false;
            
            // Determine state based on scene
            if (scene.buildIndex == GameConstants.Scenes.MainMenu)
            {
                ChangeState(GameState.MainMenu);
            }
            else
            {
                ChangeState(GameState.Gameplay);
            }
            
            EventBus.RaiseLevelLoaded(scene.buildIndex);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            IsTransitioning = true;
        }

        private void DetermineInitialState()
        {
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.buildIndex == GameConstants.Scenes.MainMenu)
            {
                CurrentState = GameState.MainMenu;
            }
            else
            {
                CurrentState = GameState.Gameplay;
            }
            ApplyStateEffects(CurrentState);
        }
        #endregion

        #region Event Subscriptions
        private void SubscribeToEvents()
        {
            EventBus.OnGamePaused += HandleGamePaused;
            EventBus.OnDialogueStateChanged += HandleDialogueStateChanged;
            EventBus.OnPlayerDied += HandlePlayerDied;
            EventBus.OnPlayerRespawn += HandlePlayerRespawn;
            EventBus.OnLevelCompleted += HandleLevelCompleted;
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.OnGamePaused -= HandleGamePaused;
            EventBus.OnDialogueStateChanged -= HandleDialogueStateChanged;
            EventBus.OnPlayerDied -= HandlePlayerDied;
            EventBus.OnPlayerRespawn -= HandlePlayerRespawn;
            EventBus.OnLevelCompleted -= HandleLevelCompleted;
        }
        #endregion

        #region State Management
        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            
            PreviousState = CurrentState;
            CurrentState = newState;
            ApplyStateEffects(newState);
            EventBus.RaiseGameStateChanged(CurrentState);
            
            #if UNITY_EDITOR
            Debug.Log($"[GameStateManager] State changed: {PreviousState} -> {CurrentState}");
            #endif
        }

        public void ReturnToPreviousState()
        {
            if (PreviousState != CurrentState) 
                ChangeState(PreviousState);
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
        public bool IsInMenu => CurrentState == GameState.MainMenu;
        public bool IsLoading => CurrentState == GameState.Loading;
        
        /// <summary>
        /// Returns true if gameplay input should be processed.
        /// </summary>
        public bool CanProcessGameplayInput => CurrentState == GameState.Gameplay;
        #endregion

        #region State Effects
        private void ApplyStateEffects(GameState state)
        {
            switch (state)
            {
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    SetCursorState(false, CursorLockMode.Locked);
                    break;
                    
                case GameState.Paused:
                    Time.timeScale = 0f;
                    SetCursorState(true, CursorLockMode.None);
                    break;
                    
                case GameState.Dialogue:
                    Time.timeScale = 0f;
                    SetCursorState(true, CursorLockMode.None);
                    break;
                    
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    SetCursorState(true, CursorLockMode.None);
                    break;
                    
                case GameState.Loading:
                    Time.timeScale = 1f; // Keep time running during loads for animations
                    SetCursorState(false, CursorLockMode.Locked);
                    break;
                    
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    SetCursorState(true, CursorLockMode.None);
                    break;
            }
        }

        private void SetCursorState(bool visible, CursorLockMode lockMode)
        {
            Cursor.visible = visible;
            Cursor.lockState = lockMode;
        }
        #endregion

        #region Event Handlers
        private void HandleGamePaused(bool isPaused)
        {
            if (IsGameOver) return; // Don't allow pause during game over
            ChangeState(isPaused ? GameState.Paused : GameState.Gameplay);
        }
        
        private void HandleDialogueStateChanged(bool isOpen)
        {
            if (IsGameOver || IsPaused) return;
            ChangeState(isOpen ? GameState.Dialogue : GameState.Gameplay);
        }
        
        private void HandlePlayerDied() => ChangeState(GameState.GameOver);
        
        private void HandlePlayerRespawn() => ChangeState(GameState.Gameplay);
        
        private void HandleLevelCompleted() => ChangeState(GameState.Loading);
        #endregion
    }
}

