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
using Gameplay.Characters.Player;
using UI.Menus;

namespace UI.Managers
{
    public sealed class UIManager : MonoBehaviour
    {
        static UIManager instance;

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

        Player player;
        InputReader inputReader;
        readonly Dictionary<string, GameObject> indicators = new();

        void Awake()
        {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            CheckSaveData();
        }

        void OnEnable()
        {
            EventBus.Subscribe<ScoreChangedEvent>(UpdateCoinDisplay);
            EventBus.Subscribe<PlayerDiedEvent>(GameOver);
            EventBus.Subscribe<PowerUpActivatedEvent>(ActivateIndicator);
            inputReader = InputReader.Instance;
            if (inputReader != null) inputReader.PauseEvent += TogglePause;
            SceneManager.sceneLoaded += OnSceneLoaded;
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
            if (mode != LoadSceneMode.Single || inputReader == null) return;
            if (scene.name == GameConfig.Default.MainMenuSceneName) inputReader.EnableUIInput();
            else inputReader.EnableGameplayInput();
        }

        void Start() => player = FindFirstObjectByType<Player>();

        void CheckSaveData()
        {
            if (SceneManager.GetActiveScene().name != GameConfig.Default.MainMenuSceneName) return;
            continueButton.gameObject.SetActive(ServiceLocator.Get<IGameManager>()?.SaveDataExists() ?? false);
        }

        public void NewGame()
        {
            ServiceLocator.Get<IGameManager>()?.ResetCoins();
            ServiceLocator.Get<IGameManager>()?.SaveGame(true);
            ServiceLocator.Get<ISceneLoader>()?.LoadScene(GameConfig.Default.FirstLevelSceneName);
        }

        public void ContinueGame() =>
            ServiceLocator.Get<ISceneLoader>()?.LoadScene(
                ServiceLocator.Get<IGameManager>()?.GetLastSavedLevelName() ?? GameConfig.Default.FirstLevelSceneName);

        public void GameOver(PlayerDiedEvent _)
        {
            if (gameOverScreen.activeInHierarchy) return;
            gameOverScreen.SetActive(true);
            SetCursor(true);
            ServiceLocator.Get<IGameStateManager>()?.ChangeState(GameState.GameOver);
            ServiceLocator.Get<IAudioManager>()?.PlaySound(gameOverSound);
        }

        public void Restart() => ServiceLocator.Get<ISceneLoader>()?.LoadScene(SceneManager.GetActiveScene().name);
        public void MainMenu() { SetCursor(true); ServiceLocator.Get<ISceneLoader>()?.LoadScene(GameConfig.Default.MainMenuSceneName); }

        public void Quit()
        {
            ServiceLocator.Get<IGameManager>()?.SaveGame();
            SetCursor(true);
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void TogglePause()
        {
            if (gameOverScreen.activeInHierarchy) return;
            bool pause = !pauseScreen.activeInHierarchy;
            pauseScreen.SetActive(pause);
            SetCursor(pause);
            if (player != null) player.enabled = !pause;
            EventBus.Raise(new GamePausedEvent(pause));
        }

        public void ShowLoadingScreen(bool show) => loadingScreen.SetActive(show);
        public void UpdateLoadingImage(float progress) => loadingImage.fillAmount = progress;
        void UpdateCoinDisplay(ScoreChangedEvent e) => coinText.SetText("{0}", e.Score);
        void SetCursor(bool visible) { Cursor.visible = visible; Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked; }

        void ActivateIndicator(PowerUpActivatedEvent e)
        {
            if (indicators.TryGetValue(e.Name, out var existing)) Destroy(existing);

            var indicator = Instantiate(indicatorPrefab, indicatorPanel);
            if (indicator.GetComponentInChildren<Image>() is { } img) img.sprite = e.Icon;
            if (indicator.GetComponentInChildren<TMP_Text>() is { } txt) txt.text = e.Name;
            indicators[e.Name] = indicator;

            _ = FadeOutAsync(e.Name, indicator, e.Duration);
        }

        async Awaitable FadeOutAsync(string name, GameObject indicator, float duration)
        {
            var img = indicator.GetComponentInChildren<Image>();
            float startAlpha = img != null ? img.color.a : 1f;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                if (indicator == null) return;
                if (img != null) img.color = new(img.color.r, img.color.g, img.color.b, Mathf.Lerp(startAlpha, 0, t / duration));
                await Awaitable.NextFrameAsync();
            }

            indicators.Remove(name);
            if (indicator != null) Destroy(indicator);
        }
    }
}
