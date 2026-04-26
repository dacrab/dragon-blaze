using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Events;
using Core.Managers;

namespace Core.State
{
    public sealed class GameStateManager : SingletonManager<GameStateManager>
    {
        [SerializeField] GameConfig gameConfig;
        
        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        public bool IsPlaying => CurrentState == GameState.Gameplay;
        public static bool IsCurrentlyPlaying => Instance is { IsPlaying: true };

        protected override void OnInit()
        {
            if (gameConfig == null) gameConfig = Resources.Load<GameConfig>("GameConfig");
                
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventBus.OnGamePaused += paused => ChangeState(paused ? GameState.Paused : GameState.Gameplay);
            EventBus.OnDialogueStateChanged += open => { if (CurrentState is GameState.Gameplay or GameState.Dialogue) ChangeState(open ? GameState.Dialogue : GameState.Gameplay); };
            EventBus.OnPlayerDied += () => ChangeState(GameState.GameOver);
            EventBus.OnPlayerRespawn += () => ChangeState(GameState.Gameplay);
            
            CurrentState = GetStateForScene(SceneManager.GetActiveScene().buildIndex);
            ApplyState();
        }

        protected override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            base.OnDestroy();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ChangeState(GetStateForScene(scene.buildIndex));
            EventBus.OnLevelLoaded?.Invoke(scene.buildIndex);
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            ApplyState();
            EventBus.OnGameStateChanged?.Invoke(CurrentState);
        }

        void ApplyState()
        {
            bool showCursor = CurrentState is not (GameState.Gameplay or GameState.Loading);
            Time.timeScale = CurrentState is GameState.Paused or GameState.Dialogue or GameState.GameOver ? gameConfig.pausedTimeScale : gameConfig.normalTimeScale;
            Cursor.visible = showCursor;
            Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        }

        GameState GetStateForScene(int buildIndex) => buildIndex == gameConfig.mainMenuSceneIndex ? GameState.MainMenu : GameState.Gameplay;
    }
}
}