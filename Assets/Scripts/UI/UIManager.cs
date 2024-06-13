using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro; // Add this to use TextMeshPro components

public class UIManager : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Level Transition")]
    [SerializeField] private GameObject loadingScreen; // Assign this in the Inspector
    [SerializeField] private Image loadingImage; // Assign this in the Inspector
    private float _target;

    [Header("Menu Options")]
    [SerializeField] private Button continueButton; // Assign in the inspector
    [SerializeField] private Button newGameButton; // Assign in the inspector

    [Header("Coin Display")]
    [SerializeField] private TextMeshProUGUI coinText; // Assign this in the inspector

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

        if (SceneManager.GetActiveScene().buildIndex == 0) // Replace 0 with your actual main menu scene build index
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
        GameManager.instance.SaveGame(true); // Assuming SaveGame method accepts a boolean for new game
        UpdateCoinDisplay(0);
        SceneManager.LoadScene("Level1"); // Ensure "Level1" is the correct scene name for your first level
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
        loadingImage.fillAmount = 0; // Initialize fill amount
        _target = 0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        // Use a smoother transition function for the fill amount.
        float fillSpeed = 0.02f; // Lower this value for a slower, more progressive fill

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            _target = progress;

            // Use a smoother interpolation function such as Lerp for a more progressive fill.
            loadingImage.fillAmount = Mathf.Lerp(loadingImage.fillAmount, _target, fillSpeed);
            Debug.Log("Coroutine running, progress: " + progress);
            yield return null;
        }
    }

    #region Game Over
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Quit()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SaveGame();
        }

        Application.Quit(); // Quits the game (only works in build)

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exits play mode (will only be executed in the editor)
#endif
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        pauseScreen.SetActive(status);
        Time.timeScale = status ? 0.01f : 1;
        Cursor.visible = status;
        Cursor.lockState = status ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
    #endregion

    public void ShowLoadingScreen(bool show)
    {
        Debug.Log("Attempting to show loading screen: " + show);
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(show);
            Debug.Log("Loading screen set to: " + show);
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
            Debug.Log("Loading Progress: " + progress + ", Fill Amount: " + loadingImage.fillAmount);
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
}
