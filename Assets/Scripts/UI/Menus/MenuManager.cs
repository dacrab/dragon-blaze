using UnityEngine;
using Core.Managers;
using Core.Input;
using UI.Managers;

namespace UI.Menus
{
    public class MenuManager : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private InputReader inputReader;
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

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.NavigateEvent += OnNavigate;
                inputReader.SubmitEvent += OnSubmit;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.NavigateEvent -= OnNavigate;
                inputReader.SubmitEvent -= OnSubmit;
            }
        }
        #endregion

        #region Event Handlers
        private void OnNavigate(Vector2 direction)
        {
            if (direction.y > 0)
                ChangePosition(-1);
            else if (direction.y < 0)
                ChangePosition(1);
        }

        private void OnSubmit()
        {
            Interact();
        }
        #endregion

        #region Public Methods
        public void ChangePosition(int _change)
        {
            currentPosition += _change;

            if (_change != 0)
                SoundManager.Instance?.PlaySound(changeSound);

            ClampPosition();
            AssignPosition();
        }
        #endregion

        #region Private Methods
        private void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
            SoundManager.Instance?.PlaySound(interactSound);

            switch (currentPosition)
            {
                case 0:
                    UIManager.Instance?.NewGame();
                    break;
                case 1:
                    UIManager.Instance?.Quit();
                    break;
            }
        }
        #endregion
    }
}
