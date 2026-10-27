using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Core.Events;
using Core.Constants;
using Core.Input;
using Core.Managers;
using Core.Services;
using Core.State;

namespace UI.Managers
{
    public sealed partial class UIManager : MonoBehaviour
    {
        static UIManager? instance;

        [Header("UI Screens")]
        [SerializeField] GameObject gameOverScreen, pauseScreen, loadingScreen;
        [SerializeField] Image loadingImage;
        [SerializeField] Button continueButton;
        [SerializeField] TextMeshProUGUI coinText;

        [Header("Audio")]
        [SerializeField] AudioClip gameOverSound;

        [Header("PowerUp Indicators")]
        [SerializeField] GameObject indicatorPrefab;
        [SerializeField] Transform indicatorPanel;

        InputReader? inputReader;
        Transform? indicatorPanelHost;
        readonly Dictionary<string, GameObject> indicators = new();

        void Awake()
        {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            // LoadingManager resolves the UI through the locator; without this the loading screen never shows.
            ServiceLocator.Register<UIManager>(this);
        }

        void OnDestroy()
        {
            if (ReferenceEquals(ServiceLocator.Get<UIManager>(), this)) ServiceLocator.Unregister<UIManager>();
            if (instance == this) instance = null;
        }

        void OnEnable()
        {
            EventBus.Subscribe<ScoreChangedEvent>(UpdateCoinDisplay);
            EventBus.Subscribe<PlayerDiedEvent>(GameOver);
            EventBus.Subscribe<PowerUpActivatedEvent>(ActivateIndicator);
            inputReader = InputReader.Instance;
            if (inputReader != null) inputReader.PauseEvent += TogglePause;
            SceneManager.sceneLoaded += OnSceneLoaded;
            // The scene is already loaded when a persistent UI first enables.
            if (gameObject.scene.isLoaded) OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        void OnDisable()
        {
            EventBus.Unsubscribe<ScoreChangedEvent>(UpdateCoinDisplay);
            EventBus.Unsubscribe<PlayerDiedEvent>(GameOver);
            EventBus.Unsubscribe<PowerUpActivatedEvent>(ActivateIndicator);
            if (inputReader != null) inputReader.PauseEvent -= TogglePause;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single) return;
            // A scene can be started directly (play from any scene, editor restart); never leave the overlay stuck on.
            ShowLoadingScreen(false);
            // Scene change owns the screen: a stale overlay from the previous scene must not survive it.
            DismissPause();
            if (gameOverScreen != null) gameOverScreen.SetActive(false);
            bool menu = GameConfig.Default.IsMenuScene(scene.name);
            if (inputReader != null)
            {
                if (menu) inputReader.EnableUIInput();
                else inputReader.EnableGameplayInput();
            }

            // The main menu is the only place "Continue" belongs.
            if (continueButton != null)
                continueButton.gameObject.SetActive(menu && (ServiceLocator.Get<IGameManager>()?.SaveDataExists() ?? false));
        }
    }
}
