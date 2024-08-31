using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private RectTransform arrow;
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private AudioClip changeSound;
    [SerializeField] private AudioClip interactSound;
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
        if (uiManager != null)
        {
            uiManager.ContinueGame();
        }
        else
        {
            Debug.LogError("UIManager not assigned in MenuManager");
        }
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
        SceneManager.LoadScene(PlayerPrefs.GetInt("level", 1));
    }

    private void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}