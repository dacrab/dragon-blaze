using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Events;
using Core.Services;

namespace Core.State
{
    public sealed class GameStateManager : MonoBehaviour, IGameStateManager
    {
        static GameStateManager instance;

        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        public bool IsPlaying => CurrentState == GameState.Gameplay;
        public static bool IsCurrentlyPlaying => instance is { IsPlaying: true };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            if (instance != null) return;
            _ = new GameObject(nameof(GameStateManager)).AddComponent<GameStateManager>();
        }

        void Awake()
        {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<IGameStateManager>(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventBus.Subscribe<GamePausedEvent>(OnPaused);
            EventBus.Subscribe<DialogueStateChangedEvent>(OnDialogue);
            EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            EventBus.Subscribe<PlayerRespawnEvent>(OnPlayerRespawn);
            CurrentState = GetStateForScene(SceneManager.GetActiveScene().name);
            ApplyState();
        }

        void OnDestroy()
        {
            if (instance != this) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventBus.Unsubscribe<GamePausedEvent>(OnPaused);
            EventBus.Unsubscribe<DialogueStateChangedEvent>(OnDialogue);
            EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
            EventBus.Unsubscribe<PlayerRespawnEvent>(OnPlayerRespawn);
            ServiceLocator.Unregister<IGameStateManager>();
            instance = null;
        }

        void OnPlayerDied(PlayerDiedEvent _) => ChangeState(GameState.GameOver);
        void OnPlayerRespawn(PlayerRespawnEvent _) => ChangeState(GameState.Gameplay);
        void OnPaused(GamePausedEvent e) => ChangeState(e.Paused ? GameState.Paused : GameState.Gameplay);

        void OnDialogue(DialogueStateChangedEvent e)
        {
            if (CurrentState is GameState.Gameplay or GameState.Dialogue)
                ChangeState(e.Open ? GameState.Dialogue : GameState.Gameplay);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single) return;
            ChangeState(GetStateForScene(scene.name));
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            ApplyState();
        }

        void ApplyState()
        {
            var settings = GameConfig.Default.GetStateSettings(CurrentState);
            Time.timeScale = settings.timeScale;
            Cursor.visible = settings.cursorVisible;
            Cursor.lockState = settings.cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        GameState GetStateForScene(string sceneName) =>
            sceneName == GameConfig.Default.MainMenuSceneName ? GameState.MainMenu : GameState.Gameplay;
    }
}
