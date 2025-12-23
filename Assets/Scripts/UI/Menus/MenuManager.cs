using UnityEngine;
using Core.Managers;
using UnityEngine.InputSystem;
using Core.Constants;
using UI.Managers;

namespace UI.Menus
{
    public class MenuManager : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private RectTransform arrow;
        [SerializeField] private RectTransform[] buttons;
        [SerializeField] private AudioClip changeSound;
        [SerializeField] private AudioClip interactSound;
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
        public void ChangePosition(int change)
        {
            currentPosition += change;
            if (change != 0) SoundManager.Instance?.PlaySound(changeSound);
            currentPosition = currentPosition < 0 ? buttons.Length - 1 : currentPosition >= buttons.Length ? 0 : currentPosition;
            arrow.position = new Vector3(arrow.position.x, buttons[currentPosition].position.y);
        }

        public void ContinueGame() => LoadingManager.LoadSpecificLevel(Core.Managers.GameManager.Instance.GetLastSavedLevelIndex());

        private void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void HandleInput()
        {
            if (Keyboard.current == null) return;
            var kb = Keyboard.current;
            if (kb.upArrowKey.wasPressedThisFrame) ChangePosition(-1);
            else if (kb.downArrowKey.wasPressedThisFrame) ChangePosition(1);
            if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame) Interact();
        }

        private void Interact()
        {
            SoundManager.Instance?.PlaySound(interactSound);
            if (currentPosition == 0)
            {
                Core.Managers.GameManager.Instance.ResetCoins();
                Core.Managers.GameManager.Instance.SaveGame(true);
                LoadingManager.LoadSpecificLevel(1);
            }
            else if (currentPosition == 1) Application.Quit();
        }
        #endregion
    }
}
