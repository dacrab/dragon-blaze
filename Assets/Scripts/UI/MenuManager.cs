using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Systems;

public class MenuManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private RectTransform arrow;
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private AudioClip changeSound;
    [SerializeField] private AudioClip interactSound;
    // Removed UIManager reference as MainMenu usually doesn't share UIManager with Gameplay unless persistent.
    // But UIManager has ContinueGame logic.
    // Let's assume UIManager is present or we use GameManager/SaveSystem directly.
    [SerializeField] private UIManager uiManager; 
    #endregion

    #region Private Fields
    private int currentPosition;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        ChangePosition(0);
        ShowCursor();
    }

    private void Update()
    {
        HandleInput();
    }
    #endregion

    #region Public Methods
    public void ChangePosition(int _change)
    {
        currentPosition += _change;

        if (_change != 0)
            SoundManager.instance.PlaySound(changeSound);

        ClampPosition();
        AssignPosition();
    }

    public void ContinueGame()
    {
        // Use SaveSystem directly or GameManager
        // Original UIManager.ContinueGame() -> LoadingManager.LoadSpecificLevel(GameManager.GetLastSavedLevelIndex())
        
        // Since we refactored GameManager, we can access last saved level directly.
        int levelIndex = GameManager.instance.GetLastSavedLevelIndex();
        
        // Use LoadingManager if available (static methods)
        // But we need to be careful if LoadingManager exists in scene.
        // Assuming LoadingManager is a persistent singleton or static helper.
        LoadingManager.LoadSpecificLevel(levelIndex);
    }
    #endregion

    #region Private Methods
    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChangePosition(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            ChangePosition(1);

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetButtonDown("Submit"))
            Interact();
    }

    private void ClampPosition()
    {
        if (currentPosition < 0)
            currentPosition = buttons.Length - 1;
        else if (currentPosition > buttons.Length - 1)
            currentPosition = 0;
    }

    private void AssignPosition()
    {
        arrow.position = new Vector3(arrow.position.x, buttons[currentPosition].position.y);
    }

    private void Interact()
    {
        SoundManager.instance.PlaySound(interactSound);

        switch (currentPosition)
        {
            case 0:
                StartGame();
                break;
            case 1:
                QuitGame();
                break;
        }
    }

    private void StartGame()
    {
        // Load level 1 (or next level). Assuming 1 is first level.
        // Or check PlayerPrefs "level" if that's legacy.
        // Refactored SaveSystem uses SaveData.currentLevel.
        
        // Assuming "Start Game" means New Game?
        // If New Game, reset data.
        GameManager.instance.ResetCoins();
        GameManager.instance.SaveGame(true);
        
        // Load level 1
        LoadingManager.LoadSpecificLevel(1);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}