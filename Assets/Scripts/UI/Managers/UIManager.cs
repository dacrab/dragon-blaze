using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Core.Events;
using Core.Constants;
using Core.Input;
using Core.Managers;
using Core.State;

namespace UI.Managers;

public sealed class UIManager : SingletonManager<UIManager>
{
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

    Gameplay.Characters.Player.Player player;

    protected override void OnInit()
    {
        gameConfig ??= Resources.Load<GameConfig>("GameConfig");
        CheckSaveData();
    }

    void OnEnable()
    {
        EventBus.OnScoreChanged += UpdateCoinDisplay;
        EventBus.OnPlayerDied += GameOver;
        if (inputReader != null) inputReader.PauseEvent += TogglePause;
    }

    void OnDisable()
    {
        EventBus.OnScoreChanged -= UpdateCoinDisplay;
        EventBus.OnPlayerDied -= GameOver;
        if (inputReader != null) inputReader.PauseEvent -= TogglePause;
    }

    void Start() => player = FindFirstObjectByType<Gameplay.Characters.Player.Player>();

    void CheckSaveData()
    {
        if (SceneManager.GetActiveScene().buildIndex != GameConstants.Scenes.MainMenu) return;
        if (continueButton != null) continueButton.gameObject.SetActive(GameManager.Instance?.SaveDataExists() ?? false);
    }

    public void NewGame()
    {
        GameManager.Instance?.ResetCoins();
        GameManager.Instance?.SaveGame(true);
        Menus.LoadingManager.LoadSpecificLevel(gameConfig != null ? gameConfig.firstLevelSceneIndex : 1);
    }

    public void ContinueGame() => Menus.LoadingManager.LoadSpecificLevel(GameManager.Instance?.GetLastSavedLevelIndex() ?? (gameConfig != null ? gameConfig.firstLevelSceneIndex : 1));

    public void GameOver()
    {
        if (gameOverScreen == null || gameOverScreen.activeInHierarchy) return;
        gameOverScreen.SetActive(true);
        SetCursor(true);
        GameStateManager.Instance?.ChangeState(GameState.GameOver);
        SoundManager.Instance?.PlaySound(gameOverSound);
    }

    public void Restart() => Menus.LoadingManager.LoadSpecificLevel(SceneManager.GetActiveScene().buildIndex);

    public void MainMenu()
    {
        SetCursor(true);
        Menus.LoadingManager.LoadSpecificLevel(GameConstants.Scenes.MainMenu);
    }

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
        if (gameOverScreen != null && gameOverScreen.activeInHierarchy) return;
        bool pause = pauseScreen != null && !pauseScreen.activeInHierarchy;
        pauseScreen?.SetActive(pause);
        SetCursor(pause);
        if (player != null) player.enabled = !pause;
        EventBus.GamePaused(pause);
    }

    public void ShowLoadingScreen(bool show) => loadingScreen?.SetActive(show);
    public void UpdateLoadingImage(float progress) { if (loadingImage != null) loadingImage.fillAmount = progress; }
    void UpdateCoinDisplay(int coins) => coinText?.SetText(string.Format(gameConfig != null ? gameConfig.coinDisplayFormat : ": {0}", coins));
    void SetCursor(bool visible) { Cursor.visible = visible; Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked; }
}
