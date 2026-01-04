using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Core.Events;
using Core.Constants;
using Core.Input;
using Core.Managers;
using Core.State;

namespace UI.Managers
{
    public class UIManager : SingletonManager<UIManager>
    {
        [Header("Screens")]
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject pauseScreen;
        [SerializeField] private GameObject loadingScreen;

        [Header("UI Elements")]
        [SerializeField] private Image loadingImage;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private TextMeshProUGUI coinText;

        [Header("Audio")]
        [SerializeField] private AudioClip gameOverSound;

        [Header("Input")]
        [SerializeField] private InputReader inputReader;

        [Header("Player")]
        [SerializeField] private Gameplay.Characters.Player.PlayerController playerController;

        public bool IsPauseScreenActive => pauseScreen != null && pauseScreen.activeInHierarchy;
        public bool IsGameOverScreenActive => gameOverScreen != null && gameOverScreen.activeInHierarchy;

        protected override void OnInitialize()
        {
            CheckSaveData();
        }

        private void OnEnable()
        {
            EventBus.OnScoreChanged += UpdateCoinDisplay;
            EventBus.OnPlayerDied += GameOver;
            if (inputReader != null) inputReader.PauseEvent += OnPauseInput;
        }

        private void OnDisable()
        {
            EventBus.OnScoreChanged -= UpdateCoinDisplay;
            EventBus.OnPlayerDied -= GameOver;
            if (inputReader != null) inputReader.PauseEvent -= OnPauseInput;
        }

        private void Start()
        {
            if (playerController == null)
                playerController = FindFirstObjectByType<Gameplay.Characters.Player.PlayerController>();
        }

        private void OnPauseInput() => PauseGame(!IsPauseScreenActive);

        private void CheckSaveData()
        {
            if (GameManager.Instance == null) return;
            if (SceneManager.GetActiveScene().buildIndex != GameConstants.Scenes.MainMenu) return;

            bool saveExists = GameManager.Instance.SaveDataExists();
            if (continueButton != null) continueButton.gameObject.SetActive(saveExists);
            if (newGameButton != null) newGameButton.gameObject.SetActive(true);
        }

        public void SaveGame() => GameManager.Instance?.SaveGame();

        public void NewGame()
        {
            GameManager.Instance.ResetCoins();
            GameManager.Instance.SaveGame(true);
            UI.Menus.LoadingManager.LoadSpecificLevel(1);
        }

        public void ContinueGame() => UI.Menus.LoadingManager.LoadSpecificLevel(GameManager.Instance.GetLastSavedLevelIndex());

        public void GameOver()
        {
            if (IsGameOverScreenActive) return;
            SetGameOverState(true);
            SoundManager.Instance?.PlaySound(gameOverSound);
        }

        public void Restart()
        {
            SetGameOverState(false);
            UI.Menus.LoadingManager.LoadSpecificLevel(SceneManager.GetActiveScene().buildIndex);
        }

        public void MainMenu()
        {
            SetGameOverState(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            UI.Menus.LoadingManager.LoadSpecificLevel(GameConstants.Scenes.MainMenu);
        }

        public void Quit()
        {
            GameManager.Instance?.SaveGame();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void PauseGame(bool status)
        {
            if (IsGameOverScreenActive) return;
            pauseScreen?.SetActive(status);
            GameStateManager.Instance?.ChangeState(status ? GameState.Paused : GameState.Gameplay);
            Cursor.visible = status;
            Cursor.lockState = status ? CursorLockMode.None : CursorLockMode.Locked;
            if (playerController != null) playerController.enabled = !status;
            EventBus.RaiseGamePaused(status);
        }

        public void ShowLoadingScreen(bool show) => loadingScreen?.SetActive(show);
        public void UpdateLoadingImage(float progress) { if (loadingImage != null) loadingImage.fillAmount = progress; }
        public void UpdateCoinDisplay(int coins) => coinText?.SetText($": {coins}");
        public void RefreshUI() => UpdateCoinDisplay(GameManager.Instance?.TotalCoins ?? 0);

        private void SetGameOverState(bool isGameOver)
        {
            gameOverScreen?.SetActive(isGameOver);
            Cursor.visible = isGameOver;
            Cursor.lockState = isGameOver ? CursorLockMode.None : CursorLockMode.Locked;
            GameStateManager.Instance?.ChangeState(isGameOver ? GameState.GameOver : GameState.Gameplay);
            if (playerController != null) playerController.enabled = !isGameOver;
        }
    }
}
