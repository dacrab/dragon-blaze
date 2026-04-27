using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Constants;
using Core.Input;
using Core.Managers;
using Core.State;

namespace UI.Managers
{

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

    [Header("PowerUp Indicators")]
    [SerializeField] GameObject indicatorPrefab;
    [SerializeField] Transform indicatorPanel;

    Gameplay.Characters.Player.Player player;
    readonly Dictionary<string, (GameObject obj, Coroutine routine)> indicators = new();

    protected override void OnInit()
    {
        gameConfig ??= Resources.Load<GameConfig>("GameConfig");
        CheckSaveData();
    }

    void OnEnable()
    {
        EventBus.OnScoreChanged += UpdateCoinDisplay;
        EventBus.OnPlayerDied += GameOver;
        EventBus.OnPowerUpActivated += ActivateIndicator;
        if (inputReader != null) inputReader.PauseEvent += TogglePause;
    }

    void OnDisable()
    {
        EventBus.OnScoreChanged -= UpdateCoinDisplay;
        EventBus.OnPlayerDied -= GameOver;
        EventBus.OnPowerUpActivated -= ActivateIndicator;
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
        GameManager.Instance?.PlaySound(gameOverSound);
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

    void ActivateIndicator(string name, Sprite icon, float duration)
    {
        if (indicatorPrefab == null || indicatorPanel == null) return;

        if (indicators.TryGetValue(name, out var existing))
        {
            if (existing.routine != null) StopCoroutine(existing.routine);
            var newRoutine = StartCoroutine(FadeOut(name, existing.obj, duration));
            indicators[name] = (existing.obj, newRoutine);
            return;
        }

        var indicator = Instantiate(indicatorPrefab, indicatorPanel);
        if (indicator.GetComponentInChildren<Image>() is { } img) img.sprite = icon;
        if (indicator.GetComponentInChildren<TMP_Text>() is { } txt) txt.text = name;

        var routine = StartCoroutine(FadeOut(name, indicator, duration));
        indicators[name] = (indicator, routine);
    }

    IEnumerator FadeOut(string name, GameObject indicator, float duration)
    {
        var img = indicator.GetComponentInChildren<Image>();
        var startAlpha = img != null ? img.color.a : 1f;
        
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            if (img != null) img.color = new(img.color.r, img.color.g, img.color.b, Mathf.Lerp(startAlpha, 0, t / duration));
            yield return null;
        }
        
        indicators.Remove(name);
        Destroy(indicator);
    }
}
}