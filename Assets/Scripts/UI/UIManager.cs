using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header ("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;

    [Header("Menu Slider")]
    [SerializeField] private GameObject loadingScreen;
     [SerializeField] private GameObject mainMenu;



    public void Play(string levelToLoad)
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is beyond the total scene count
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("You've reached the last level!");
            return;
        }

        SceneManager.LoadScene(nextSceneIndex);
        loadingScreen.SetActive(true);
        mainMenu.SetActive(false);

        StartCoroutine(LoadLevelASync(levelToLoad));
    }
    
    
    IEnumerator LoadLevelASync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
    private void Awake()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //If pause screen already active unpause and viceversa
            PauseGame(!pauseScreen.activeInHierarchy);
        }
    }

    #region Game Over
    //Activate game over screen
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
        Time.timeScale = 0;
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
            Time.timeScale = 0;
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
}