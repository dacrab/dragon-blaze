using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Core.Events;
using Core.Constants;
using Core.Managers;
using Core.State;
using Core.Services;
using Gameplay.Characters.Player;
using UI.Menus;
using UnityEngine.InputSystem;

namespace UI.Managers
{
    public class UIManager : SingletonManager<UIManager>
    {

        #region Serialized Fields
        [Header("Screens")]
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject pauseScreen;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Image loadingBarFill;

        [Header("UI Elements")]
        [SerializeField] private Image loadingImage;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private TextMeshProUGUI coinText;

        [Header("Audio")]
        [SerializeField] private AudioClip gameOverSound;

        [Header("Player Reference")]
        [SerializeField] private PlayerController playerController;
        #endregion

        #region Properties
        public bool IsPauseScreenActive => pauseScreen.activeInHierarchy;
        public bool IsGameOverScreenActive => gameOverScreen.activeInHierarchy;
        #endregion

        #region Initialization
        protected override void OnInitialize()
        {
            base.OnInitialize();
            ServiceLocator.Register<UIManager>(this);
            CheckSaveData();
        }

        protected override void OnShutdown()
        {
            UnsubscribeFromEvents();
            ServiceLocator.Unregister<UIManager>();
            base.OnShutdown();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            EventBus.OnScoreChanged += UpdateCoinDisplay;
            EventBus.OnPlayerDied += GameOver;
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.OnScoreChanged -= UpdateCoinDisplay;
            EventBus.OnPlayerDied -= GameOver;
        }

        private void Update()
        {
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
                PauseGame(!IsPauseScreenActive);
        }
        #endregion

        #region Save Data Methods
        private void CheckSaveData()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager instance is not initialized");
                return;
            }

            if (SceneManager.GetActiveScene().buildIndex != GameConstants.Scenes.MainMenu) return;

            bool saveExists = GameManager.Instance.SaveDataExists();
            SetButtonVisibility(continueButton, saveExists, "Continue button");
            SetButtonVisibility(newGameButton, true, "New Game button");
        }

        private void SetButtonVisibility(Button button, bool isVisible, string buttonName)
        {
            if (button != null) button.gameObject.SetActive(isVisible);
            else Debug.LogWarning($"{buttonName} is not assigned in the Inspector.");
        }

        public void SaveGame() => GameManager.Instance?.SaveGame();
        #endregion

        #region Game Flow Methods
        public void NewGame()
        {
            GameManager.Instance.ResetCoins();
            GameManager.Instance.SaveGame(true);
            LoadingManager.LoadSpecificLevel(1);
        }

        public void ContinueGame() => LoadingManager.LoadSpecificLevel(GameManager.Instance.GetLastSavedLevelIndex());

        public void GameOver()
        {
            if (IsGameOverScreenActive) return; 
            
            SetGameOverState(true);
            SoundManager.Instance?.PlaySound(gameOverSound);
        }

        public void Restart()
        {
            SetGameOverState(false);
            LoadingManager.LoadSpecificLevel(SceneManager.GetActiveScene().buildIndex);
        }

        public void MainMenu()
        {
            SetGameOverState(false);
            ShowCursor();
            LoadingManager.LoadSpecificLevel(GameConstants.Scenes.MainMenu);
        }

        public void Quit()
        {
            GameManager.Instance?.SaveGame();
            ShowCursor();
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void PauseGame(bool status)
        {
            if (IsGameOverScreenActive) return;

            pauseScreen.SetActive(status);
            
            GameStateManager.Instance?.ChangeState(status ? GameState.Paused : GameState.Gameplay);
            
            Cursor.visible = status;
            Cursor.lockState = status ? CursorLockMode.None : CursorLockMode.Locked;
            TogglePlayerMovement(!status);
            
            EventBus.RaiseGamePaused(status);
        }
        #endregion

        #region UI Update Methods
        public void ShowLoadingScreen(bool show) => loadingScreen?.SetActive(show);
        public void UpdateLoadingImage(float progress) => loadingImage.fillAmount = progress;
        public void UpdateCoinDisplay(int coins) => coinText?.SetText($": {coins}");

        public void RefreshUI() => UpdateCoinDisplay(GameManager.Instance?.TotalCoins ?? 0);

        private void SetGameOverState(bool isGameOver)
        {
            gameOverScreen.SetActive(isGameOver);
            Cursor.visible = isGameOver;
            Cursor.lockState = isGameOver ? CursorLockMode.None : CursorLockMode.Locked;
            
            GameStateManager.Instance?.ChangeState(isGameOver ? GameState.GameOver : GameState.Gameplay);
            
            TogglePlayerMovement(!isGameOver);
        }
        #endregion

        #region Utility Methods
        private void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void TogglePlayerMovement(bool enable)
        {
            if (playerController != null)
                playerController.enabled = enable;
        }
        #endregion

    }
}
