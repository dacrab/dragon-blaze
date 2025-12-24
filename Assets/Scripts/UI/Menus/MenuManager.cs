using UnityEngine;
using Core.Managers;
using Core.Input;
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
        [SerializeField] private InputReader inputReader;
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
                inputReader.SubmitEvent += Interact;
                inputReader.EnableUIInput();
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.NavigateEvent -= OnNavigate;
                inputReader.SubmitEvent -= Interact;
            }
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

        public void ContinueGame() => LoadingManager.LoadSpecificLevel(GameManager.Instance.GetLastSavedLevelIndex());
        #endregion

        #region Private Methods
        private void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnNavigate(Vector2 direction)
        {
            if (direction.y > 0.5f) ChangePosition(-1);
            else if (direction.y < -0.5f) ChangePosition(1);
        }

        private void Interact()
        {
            SoundManager.Instance?.PlaySound(interactSound);
            if (currentPosition == 0)
            {
                GameManager.Instance.ResetCoins();
                GameManager.Instance.SaveGame(true);
                LoadingManager.LoadSpecificLevel(1);
            }
            else if (currentPosition == 1) Application.Quit();
        }
        #endregion
    }
}
