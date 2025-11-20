using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Core.Events;
using Core.Constants;
using Core.Managers;
using Gameplay.Characters.Player;
using UI.Menus; // For LoadingManager
using Core.Input;

namespace UI.Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Singleton
        public static UIManager instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }
        #endregion

        #region Serialized Fields
        [Header("Dependencies")]
        [SerializeField] private InputReader inputReader;

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

        #region Unity Lifecycle Methods
        private void OnEnable()
        {
            EventBus.OnScoreChanged += UpdateCoinDisplay;
            EventBus.OnPlayerDied += GameOver;
            
            if (inputReader != null)
            {
                inputReader.PauseEvent += OnPause;
            }
        }

        private void OnDisable()
        {
            EventBus.OnScoreChanged -= UpdateCoinDisplay;
            EventBus.OnPlayerDied -= GameOver;
            
            if (inputReader != null)
            {
                inputReader.PauseEvent -= OnPause;
            }
        }

        private void Start() => CheckSaveData();
        #endregion

        #region Input Handling
        private void OnPause()
        {
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
            if (button != null)
            {
                button.gameObject.SetActive(isVisible);
            }
            else
            {
                Debug.LogWarning($"{buttonName} is not assigned in the Inspector.");
            }
        }

        public void SaveGame()
        {
            GameManager.Instance?.SaveGame();
        }
        #endregion

        #region Game Flow Methods
        public void NewGame()
        {
            GameManager.Instance.ResetCoins();
            GameManager.Instance.SaveGame(true);
            
            int levelToLoad = 1;
            LoadingManager.LoadSpecificLevel(levelToLoad);
        }

        public void ContinueGame()
        {
            int lastSavedLevelIndex = GameManager.Instance.GetLastSavedLevelIndex();
            LoadingManager.LoadSpecificLevel(lastSavedLevelIndex);
        }

        public void GameOver()
        {
            if (IsGameOverScreenActive) return; 
            
            SetGameOverState(true);
            SoundManager.instance.PlaySound(gameOverSound);
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
            Time.timeScale = status ? 0.01f : 1;
            Cursor.visible = status;
            Cursor.lockState = status ? CursorLockMode.None : CursorLockMode.Locked;
            TogglePlayerMovement(!status);
            
            EventBus.RaiseGamePaused(status);
        }
        #endregion

        #region UI Update Methods
        public void ShowLoadingScreen(bool show)
        {
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(show);
            }
            else
            {
                CreateTemporaryLoadingScreen(show);
            }
        }

        public void UpdateLoadingImage(float progress)
        {
            if (loadingImage != null)
            {
                loadingImage.fillAmount = progress;
            }
        }

        public void UpdateCoinDisplay(int coins)
        {
            if (coinText != null)
            {
                coinText.text = $": {coins}";
            }
        }

        public void RefreshUI()
        {
            if (GameManager.Instance != null)
            {
                UpdateCoinDisplay(GameManager.Instance.TotalCoins);
            }
        }

        private void SetGameOverState(bool isGameOver)
        {
            gameOverScreen.SetActive(isGameOver);
            Cursor.visible = isGameOver;
            Cursor.lockState = isGameOver ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = isGameOver ? 0 : 1;
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

        #region Helper Methods
        private void CreateTemporaryLoadingScreen(bool show)
        {
            if (show)
            {
                GameObject tempLoadingScreen = new GameObject("Temporary Loading Screen");
                Canvas canvas = tempLoadingScreen.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999;

                Image backgroundImage = tempLoadingScreen.AddComponent<Image>();
                backgroundImage.color = new Color(0, 0, 0, 0.5f);

                GameObject loadingTextObj = new GameObject("Loading Text");
                loadingTextObj.transform.SetParent(tempLoadingScreen.transform, false);
                Text loadingText = loadingTextObj.AddComponent<Text>();
                loadingText.text = "Loading...";
                loadingText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                loadingText.fontSize = 24;
                loadingText.color = Color.white;
                loadingText.alignment = TextAnchor.MiddleCenter;

                RectTransform rectTransform = loadingText.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;

                loadingScreen = tempLoadingScreen;
            }
            else if (loadingScreen != null)
            {
                Destroy(loadingScreen);
                loadingScreen = null;
            }
        }
        #endregion
    }
}
