using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Events;
using Core.Managers;

namespace Core.State
{
    public class GameStateManager : SingletonManager<GameStateManager>
    {
        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        public bool IsPlaying => CurrentState == GameState.Gameplay;
        public static bool IsCurrentlyPlaying => Instance != null && Instance.IsPlaying;

        protected override void OnInitialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventBus.OnGamePaused += HandleGamePaused;
            EventBus.OnDialogueStateChanged += HandleDialogueStateChanged;
            EventBus.OnPlayerDied += () => ChangeState(GameState.GameOver);
            EventBus.OnPlayerRespawn += () => ChangeState(GameState.Gameplay);
            EventBus.OnLevelCompleted += () => ChangeState(GameState.Loading);
            
            // Set initial state
            CurrentState = SceneManager.GetActiveScene().buildIndex == GameConstants.Scenes.MainMenu 
                ? GameState.MainMenu : GameState.Gameplay;
            ApplyStateEffects(CurrentState);
        }

        protected override void OnShutdown()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventBus.OnGamePaused -= HandleGamePaused;
            EventBus.OnDialogueStateChanged -= HandleDialogueStateChanged;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ChangeState(scene.buildIndex == GameConstants.Scenes.MainMenu ? GameState.MainMenu : GameState.Gameplay);
            EventBus.RaiseLevelLoaded(scene.buildIndex);
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            ApplyStateEffects(newState);
            EventBus.RaiseGameStateChanged(CurrentState);
        }

        private void ApplyStateEffects(GameState state)
        {
            bool showCursor = state != GameState.Gameplay && state != GameState.Loading;
            Time.timeScale = (state == GameState.Paused || state == GameState.Dialogue || state == GameState.GameOver) ? 0f : 1f;
            Cursor.visible = showCursor;
            Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        }

        private void HandleGamePaused(bool isPaused)
        {
            if (CurrentState == GameState.GameOver) return;
            ChangeState(isPaused ? GameState.Paused : GameState.Gameplay);
        }

        private void HandleDialogueStateChanged(bool isOpen)
        {
            if (CurrentState == GameState.GameOver || CurrentState == GameState.Paused) return;
            ChangeState(isOpen ? GameState.Dialogue : GameState.Gameplay);
        }
    }
}
