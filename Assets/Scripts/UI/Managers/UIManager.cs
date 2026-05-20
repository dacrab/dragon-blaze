using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Core.Events;
using Core.Constants;
using Core.Input;
using Core.Managers;
using Core.State;

namespace UI.Managers
{
    public sealed class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Screens")]
        [SerializeField] GameObject gameOverScreen, pauseScreen, loadingScreen;
        [SerializeField] Image loadingImage;
        [SerializeField] Button continueButton;
        [SerializeField] TextMeshProUGUI coinText;

        [Header("Audio")]
        [SerializeField] AudioClip gameOverSound;

        [Header("Input")]
        [SerializeField] InputReader inputReader;

        [Header("Config")]
        [SerializeField] GameConfig gameConfig;

        [Header("PowerUp Indicators")]
        [SerializeField] GameObject indicatorPrefab;
        [SerializeField] Transform indicatorPanel;

        Gameplay.Characters.Player.Player player;
        readonly Dictionary<string, GameObject> indicators = new();

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CheckSaveData();
        }

        void OnEnable()
        {
            EventBus.OnScoreChanged += UpdateCoinDisplay;
            EventBus.OnPlayerDied += GameOver;
            EventBus.OnPowerUpActivated += ActivateIndicator;
            inputReader.PauseEvent += TogglePause;
        }

        void OnDisable()
        {
            EventBus.OnScoreChanged -= UpdateCoinDisplay;
            EventBus.OnPlayerDied -= GameOver;
            EventBus.OnPowerUpActivated -= ActivateIndicator;
            inputReader.PauseEvent -= TogglePause;
        }

        void Start() => player = FindFirstObjectByType<Gameplay.Characters.Player.Player>();

        void CheckSaveData()
        {
            if (SceneManager.GetActiveScene().buildIndex != GameConstants.Scenes.MainMenu) return;
            continueButton.gameObject.SetActive(GameManager.Instance?.SaveDataExists() ?? false);
        }

        public void NewGame()
        {
            GameManager.Instance?.ResetCoins();
            GameManager.Instance?.SaveGame(true);
            Menus.LoadingManager.LoadSpecificLevel(gameConfig.firstLevelSceneIndex);
        }

        public void ContinueGame() =>
            Menus.LoadingManager.LoadSpecificLevel(GameManager.Instance?.GetLastSavedLevelIndex() ?? gameConfig.firstLevelSceneIndex);

        public void GameOver()
        {
            if (gameOverScreen.activeInHierarchy) return;
            gameOverScreen.SetActive(true);
            SetCursor(true);
            GameStateManager.Instance?.ChangeState(GameState.GameOver);
            GameManager.Instance?.PlaySound(gameOverSound);
        }

        public void Restart() => Menus.LoadingManager.LoadSpecificLevel(SceneManager.GetActiveScene().buildIndex);
        public void MainMenu() { SetCursor(true); Menus.LoadingManager.LoadSpecificLevel(GameConstants.Scenes.MainMenu); }

        public void Quit()
        {
            GameManager.Instance?.SaveGame();
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
            EventBus.RaiseGamePaused(pause);
        }

        public void ShowLoadingScreen(bool show) => loadingScreen.SetActive(show);
        public void UpdateLoadingImage(float progress) => loadingImage.fillAmount = progress;
        void UpdateCoinDisplay(int coins) => coinText.SetText(string.Format(gameConfig.coinDisplayFormat, coins));
        void SetCursor(bool visible) { Cursor.visible = visible; Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked; }

        async void ActivateIndicator(string name, Sprite icon, float duration)
        {
            if (indicators.TryGetValue(name, out var existing)) Destroy(existing);

            var indicator = Instantiate(indicatorPrefab, indicatorPanel);
            if (indicator.GetComponentInChildren<Image>() is { } img) img.sprite = icon;
            if (indicator.GetComponentInChildren<TMP_Text>() is { } txt) txt.text = name;
            indicators[name] = indicator;

            await FadeOutAsync(name, indicator, duration);
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
