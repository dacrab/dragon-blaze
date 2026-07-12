using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Events;

namespace Core.State
{
    public sealed class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        [SerializeField] GameConfig gameConfig;

        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        public bool IsPlaying => CurrentState == GameState.Gameplay;
        public static bool IsCurrentlyPlaying => Instance is { IsPlaying: true };

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventBus.OnGamePaused += OnPaused;
            EventBus.OnDialogueStateChanged += OnDialogue;
            EventBus.OnPlayerDied += OnPlayerDied;
            EventBus.OnPlayerRespawn += OnPlayerRespawn;
            CurrentState = GetStateForScene(SceneManager.GetActiveScene().buildIndex);
            ApplyState();
        }

        void OnDestroy()
        {
            if (Instance != this) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventBus.OnGamePaused -= OnPaused;
            EventBus.OnDialogueStateChanged -= OnDialogue;
            EventBus.OnPlayerDied -= OnPlayerDied;
            EventBus.OnPlayerRespawn -= OnPlayerRespawn;
            Instance = null;
        }

        void OnPlayerDied() => ChangeState(GameState.GameOver);
        void OnPlayerRespawn() => ChangeState(GameState.Gameplay);
        void OnPaused(bool paused) => ChangeState(paused ? GameState.Paused : GameState.Gameplay);

        void OnDialogue(bool open)
        {
            if (CurrentState is GameState.Gameplay or GameState.Dialogue)
                ChangeState(open ? GameState.Dialogue : GameState.Gameplay);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            ChangeState(GetStateForScene(scene.buildIndex));
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            ApplyState();
        }

        void ApplyState()
        {
            bool showCursor = CurrentState is not (GameState.Gameplay or GameState.Loading);
            Time.timeScale = CurrentState is GameState.Paused or GameState.Dialogue or GameState.GameOver
                ? gameConfig.pausedTimeScale : gameConfig.normalTimeScale;
            Cursor.visible = showCursor;
            Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        }

        GameState GetStateForScene(int buildIndex) =>
            buildIndex == gameConfig.mainMenuSceneIndex ? GameState.MainMenu : GameState.Gameplay;
    }
}
