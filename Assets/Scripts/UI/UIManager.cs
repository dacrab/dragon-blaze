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

    [Header(" Level Transition")]
    [SerializeField] private ParticleSystem particles;
    public GameObject loadingScreen; // Assign this in the Inspector
    public Image loadingBar;
    private float _target;

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
        // Check if save data exists using GameManager
        bool saveExists = GameManager.instance.SaveDataExists();
        continueButton.gameObject.SetActive(saveExists); // Only activate continue button if save exists
        newGameButton.gameObject.SetActive(true); // Always activate new game button
    }

    public void NewGame()
    {
        // Reset or create new save data
        GameManager.instance.ResetCoins(); // Reset coins to 0
        GameManager.instance.SaveGame(); // Save the reset state
        SceneManager.LoadScene("Level1"); // Assuming "Level1" is your first level scene
    }

    public void ContinueGame()
    {
        // Load the existing game
        SaveData saveData = GameManager.instance.LoadGame(); // Load game data
        if (saveData != null)
        {
            SceneManager.LoadScene(saveData.currentLevel); // Load the saved level
        }
        else
        {
            Debug.Log("No save data found, cannot continue!");
            // Optionally, you could also handle this case by showing an error message to the user
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
        loadingBar.fillAmount = Mathf.MoveTowards(loadingBar.fillAmount , _target,3 *Time.deltaTime);
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
            loadingScreen.SetActive(show);
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
}