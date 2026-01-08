using UnityEngine;
using UnityEngine.Events;
using Core.Managers;
using Core.Constants;
using Core.Input;

namespace UI.Menus;

[System.Serializable]
public class MenuAction
{
    public string name;
    public UnityEvent action;
}

public sealed class MenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] RectTransform arrow;
    [SerializeField] RectTransform[] buttons;
    
    [Header("Audio")]
    [SerializeField] AudioClip changeSound, interactSound;
    
    [Header("Input")]
    [SerializeField] InputReader inputReader;
    
    [Header("Config")]
    [SerializeField] GameConfig gameConfig;
    
    [Header("Menu Actions")]
    [SerializeField] MenuAction[] menuActions;

    int currentIndex;

    void Awake()
    {
        gameConfig ??= Resources.Load<GameConfig>("GameConfig");
        UpdateArrow();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnEnable()
    {
        if (inputReader == null) return;
        inputReader.NavigateEvent += OnNavigate;
        inputReader.SubmitEvent += OnSubmit;
        inputReader.EnableUIInput();
    }

    void OnDisable()
    {
        if (inputReader == null) return;
        inputReader.NavigateEvent -= OnNavigate;
        inputReader.SubmitEvent -= OnSubmit;
    }

    void OnNavigate(Vector2 dir)
    {
        float threshold = gameConfig != null ? gameConfig.navigationThreshold : 0.5f;
        if (dir.y > threshold) ChangeIndex(-1);
        else if (dir.y < -threshold) ChangeIndex(1);
    }

    void ChangeIndex(int delta)
    {
        currentIndex = (currentIndex + delta + buttons.Length) % buttons.Length;
        SoundManager.Instance?.PlaySound(changeSound);
        UpdateArrow();
    }

    void UpdateArrow() => arrow.position = new(arrow.position.x, buttons[currentIndex].position.y, arrow.position.z);

    void OnSubmit()
    {
        SoundManager.Instance?.PlaySound(interactSound);
        
        if (menuActions != null && currentIndex < menuActions.Length)
            menuActions[currentIndex].action?.Invoke();
        else
            ExecuteLegacyAction();
    }

    void ExecuteLegacyAction()
    {
        switch (currentIndex)
        {
            case 0: StartNewGame(); break;
            case 1: QuitGame(); break;
        }
    }

    public void StartNewGame()
    {
        GameManager.Instance?.ResetCoins();
        GameManager.Instance?.SaveGame(true);
        LoadingManager.LoadSpecificLevel(gameConfig != null ? gameConfig.firstLevelSceneIndex : 1);
    }

    public void QuitGame() => Application.Quit();
}
