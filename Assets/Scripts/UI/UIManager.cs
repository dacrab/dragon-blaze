using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro; // Add this to use TextMeshPro components

public class UIManager : MonoBehaviour
{
    [Header ("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Level Transition")]
    public GameObject loadingScreen; // Assign this in the Inspector
    public Image loadingBar;
    private float _target;
    public Image loadingImage; // Assign this in the Inspector

    [Header("Menu Options")]
    public Button continueButton; // Assign in the inspector
    public Button newGameButton; // Assign in the inspector

    [Header("Coin Display")]
    [SerializeField] private TextMeshProUGUI coinText; // Assign this in the inspector

    private void Start()
    {
        CheckSaveData();
    }

    private void CheckSaveData()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is not initialized");
            return;
        }

        // Check if the current scene is the main menu using the build index
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
        // Reset coins to 0
        GameManager.instance.ResetCoins();

        // Save the reset state with the first level set
        GameManager.instance.SaveGame(true); // Assuming SaveGame method accepts a boolean for new game

        // Update the UI to show the reset coin count
        UpdateCoinDisplay(0);

        // Load the first level
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

    IEnumerator LoadAsync(string sceneIndex)
    {
        loadingScreen.SetActive(true); // Activate loading screen
        loadingBar.fillAmount = 0;
        _target = 0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            _target = progress;
            loadingBar.fillAmount = Mathf.MoveTowards(loadingBar.fillAmount, _target, 3 * Time.deltaTime);
            UpdateLoadingImage(progress); // Update the loading image
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //If pause screen already active unpause and viceversa
            PauseGame(!pauseScreen.activeInHierarchy);
        }

        // Check if loadingBar is not null before trying to update its fillAmount
        if (loadingBar != null)
        {
            loadingBar.fillAmount = Mathf.MoveTowards(loadingBar.fillAmount, _target, 3 * Time.deltaTime);
        }
    }

    #region Game Over
    //Activate game over screen
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //Restart level
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
         Time.timeScale = 1;
    }

    //Main Menu
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //Quit game/exit play mode if in Editor
    public void Quit()
    {
        // Save the game before quitting
        if (GameManager.instance != null)
        {
            GameManager.instance.SaveGame();
        }

        Application.Quit(); //Quits the game (only works in build)

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Exits play mode (will only be executed in the editor)
#endif
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        
        //If status == true pause | if status == false unpause
        pauseScreen.SetActive(status);

        //When pause status is true change timescale to 0 (time stops)
        //when it's false change it back to 1 (time goes by normally)
        if (status)
        {
            Time.timeScale = 0.01f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
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
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(show);
            Debug.Log("Loading screen set to: " + show);
        }
    }

    public void UpdateLoadingImage(float progress)
    {
        if (loadingImage != null)
        {
            loadingImage.fillAmount = progress;
            Debug.Log("Loading Progress: " + progress); // Debug statement
        }
        else
        {
            Debug.LogError("Loading Image not assigned in the Inspector");
        }
    }

    public void SaveGame()
    {
        if (GameManager.instance != null)
            GameManager.instance.SaveGame();
    }

    public void UpdateCoinDisplay(int coins)
    {
        if (coinText != null)
            coinText.text = ": " + coins;
        else
            Debug.LogError("Coin TextMeshProUGUI not assigned in UIManager.");
    }

    public void RefreshUI()
    {
        if (GameManager.instance != null)
        {
            UpdateCoinDisplay(GameManager.instance.TotalCoins);
        }
    }
}
