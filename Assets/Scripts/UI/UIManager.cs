using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private PlayerMovement playerMovement;
    #endregion

    #region Properties
    public bool IsPauseScreenActive => pauseScreen.activeInHierarchy;
    public bool IsGameOverScreenActive => gameOverScreen.activeInHierarchy;
    #endregion

    #region Unity Lifecycle Methods
    private void Start() => CheckSaveData();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!IsPauseScreenActive);
        }
    }
    #endregion

    #region Save Data Methods
    private void CheckSaveData()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is not initialized");
            return;
        }

        if (SceneManager.GetActiveScene().buildIndex != 0) return;

        bool saveExists = GameManager.instance.SaveDataExists();

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
        GameManager.instance?.SaveGame();
    }
    #endregion

    #region Game Flow Methods
    public void NewGame()
    {
        GameManager.instance.ResetCoins();
        GameManager.instance.SaveGame(true);
        UpdateCoinDisplay(0);
        StartCoroutine(LoadNewGameByIndex());
    }

    public void ContinueGame()
    {
        int lastSavedLevelIndex = GameManager.instance.GetLastSavedLevelIndex();
        LoadingManager.LoadSpecificLevel(lastSavedLevelIndex);
    }

    public void Play(string sceneIndex) => StartCoroutine(LoadAsync(sceneIndex));

    public void GameOver()
    {
        SetGameOverState(true);
        SoundManager.instance.PlaySound(gameOverSound);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SetGameOverState(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        SetGameOverState(false);
        ShowCursor();
    }

    public void Quit()
    {
        GameManager.instance?.SaveGame();
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
    }
    #endregion

    #region UI Update Methods
    public void SoundVolume() => SoundManager.instance.ChangeSoundVolume(0.2f);

    public void MusicVolume() => SoundManager.instance.ChangeMusicVolume(0.2f);

    public void ShowLoadingScreen(bool show)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(show);
        }
        else
        {
            Debug.LogWarning("Loading screen GameObject is not assigned in the Inspector. Creating a temporary one.");
            CreateTemporaryLoadingScreen(show);
        }
    }

    public void UpdateLoadingImage(float progress)
    {
        if (loadingImage != null)
        {
            loadingImage.fillAmount = progress;
        }
        else
        {
            Debug.LogError("Loading Image not assigned in the Inspector");
        }
    }

    public void UpdateCoinDisplay(int coins)
    {
        if (coinText != null)
        {
            coinText.text = $": {coins}";
        }
        else
        {
            Debug.LogError("Coin TextMeshProUGUI not assigned in UIManager.");
        }
    }

    public void RefreshUI()
    {
        if (GameManager.instance != null)
        {
            UpdateCoinDisplay(GameManager.instance.TotalCoins);
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
        if (playerMovement != null)
            playerMovement.enabled = enable;
    }
    #endregion

    #region Coroutines
    private IEnumerator LoadNewGameByIndex()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        ShowLoadingScreen(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            UpdateLoadingImage(progress);

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        ShowLoadingScreen(false);
    }

    private IEnumerator LoadAsync(string sceneIndex)
    {
        loadingScreen.SetActive(true);
        loadingImage.fillAmount = 0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float simulatedProgress = 0f;
        float fillSpeed = 0.1f;
        float simulatedDuration = 3f;

        while (simulatedProgress < 1f)
        {
            simulatedProgress += Time.deltaTime / simulatedDuration;
            loadingImage.fillAmount = Mathf.Lerp(loadingImage.fillAmount, simulatedProgress, fillSpeed);
            yield return null;
        }

        loadingImage.fillAmount = 1f;
        yield return new WaitForSeconds(1);
        operation.allowSceneActivation = true;
        yield return new WaitUntil(() => operation.isDone);
        loadingScreen.SetActive(false);
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