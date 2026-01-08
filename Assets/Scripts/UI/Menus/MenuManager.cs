using UnityEngine;
using UnityEngine.Events;
using Core.Managers;
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
    
    [Header("Settings")]
    [SerializeField] float navigationThreshold = 0.5f;
    [SerializeField] int firstLevelIndex = 1;
    
    [Header("Menu Actions")]
    [SerializeField] MenuAction[] menuActions;

    int currentIndex;

    void Awake()
    {
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
        if (dir.y > navigationThreshold) ChangeIndex(-1);
        else if (dir.y < -navigationThreshold) ChangeIndex(1);
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
        {
            menuActions[currentIndex].action?.Invoke();
        }
        else
        {
            // Fallback for legacy behavior
            ExecuteLegacyAction();
        }
    }

    void ExecuteLegacyAction()
    {
        switch (currentIndex)
        {
            case 0:
                StartNewGame();
                break;
            case 1:
                QuitGame();
                break;
        }
    }

    public void StartNewGame()
    {
        GameManager.Instance?.ResetCoins();
        GameManager.Instance?.SaveGame(true);
        LoadingManager.LoadSpecificLevel(firstLevelIndex);
    }

    public void QuitGame() => Application.Quit();
}