using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Level Transition")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingImage;

    [Header("Menu Options")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;

    [Header("Coin Display")]
    [SerializeField] private TextMeshProUGUI coinText;

    public PlayerMovement playerMovement; // Assign this in the Unity Editor

    private void Start()
    {
        CheckSaveData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!pauseScreen.activeInHierarchy);
        }
    }

    private void CheckSaveData()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is not initialized");
            return;
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            bool saveExists = GameManager.instance.SaveDataExists();

            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(saveExists);
            }
            else
            {
                Debug.LogWarning("Continue button is not assigned in the Inspector.");
            }

            if (newGameButton != null)
            {
                newGameButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("New Game button is not assigned in the Inspector.");
            }
        }
    }

    public void NewGame()
    {
        GameManager.instance.ResetCoins();
        GameManager.instance.SaveGame(true);
        UpdateCoinDisplay(0);
        SceneManager.LoadScene("Level1");
    }

    public void ContinueGame()
    {
        SaveData saveData = GameManager.instance.LoadGame();
        if (saveData != null)
        {
            Debug.Log("Loaded save data with level index: " + saveData.currentLevel);
            SceneManager.LoadScene(saveData.currentLevel);
        }
        else
        {
            Debug.LogError("No save data found, cannot continue!");
        }
    }

    public void Play(string sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    private IEnumerator LoadAsync(string sceneIndex)
    {
        loadingScreen.SetActive(true);
        loadingImage.fillAmount = 0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float simulatedProgress = 0f;
        float fillSpeed = 0.1f; // Speed of the simulated loading progress
        float simulatedDuration = 3f; // Simulated duration for loading

        while (simulatedProgress < 1f)
        {
            simulatedProgress += Time.deltaTime / simulatedDuration;
            loadingImage.fillAmount = Mathf.Lerp(loadingImage.fillAmount, simulatedProgress, fillSpeed);
            Debug.Log("Current fill amount: " + loadingImage.fillAmount);
            yield return null;
        }

        loadingImage.fillAmount = 1f;
        Debug.Log("Loading complete. Fill amount: " + loadingImage.fillAmount);

        yield return new WaitForSeconds(1); // Optional delay after filling
        operation.allowSceneActivation = true;
        yield return new WaitUntil(() => operation.isDone);
        loadingScreen.SetActive(false);
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ShowGameOverScreen(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        ShowGameOverScreen(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ShowGameOverScreen(false);
    }

    public void Quit()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SaveGame();
        }

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void PauseGame(bool status)
    {
        if (gameOverScreen.activeInHierarchy)
        {
            return; // Do not allow pausing if the game over screen is active
        }

        pauseScreen.SetActive(status);
        Time.timeScale = status ? 0.01f : 1;
        Cursor.visible = status;
        Cursor.lockState = status ? CursorLockMode.None : CursorLockMode.Locked;
        ShowPauseScreen(status);
    }

    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }

    public void ShowLoadingScreen(bool show)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(show);
        }
        else
        {
            Debug.LogError("Loading screen GameObject is not assigned in the Inspector");
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

    public void SaveGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SaveGame();
        }
    }

    public void UpdateCoinDisplay(int coins)
    {
        if (coinText != null)
        {
            coinText.text = ": " + coins;
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

    public bool IsPauseScreenActive()
    {
        return pauseScreen.activeInHierarchy;
    }

    public bool IsGameOverScreenActive()
    {
        return gameOverScreen.activeInHierarchy;
    }

    public void TogglePlayerMovement(bool enable)
    {
        if (playerMovement != null)
            playerMovement.enabled = enable;
    }

    public void ShowPauseScreen(bool show)
    {
        pauseScreen.SetActive(show);
        TogglePlayerMovement(!show);
    }

    public void ShowGameOverScreen(bool show)
    {
        gameOverScreen.SetActive(show);
        TogglePlayerMovement(!show);
    }
}