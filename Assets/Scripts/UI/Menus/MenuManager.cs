using UnityEngine;
using Core.Managers;
using Core.Input;
using Core.Constants;

namespace UI.Menus
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private RectTransform arrow;
        [SerializeField] private RectTransform[] buttons;
        [SerializeField] private AudioClip changeSound;
        [SerializeField] private AudioClip interactSound;
        [SerializeField] private InputReader inputReader;

        private int currentPosition;

        private void Awake()
        {
            ChangePosition(0);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnEnable()
        {
            if (inputReader == null) return;
            inputReader.NavigateEvent += OnNavigate;
            inputReader.SubmitEvent += Interact;
            inputReader.EnableUIInput();
        }

        private void OnDisable()
        {
            if (inputReader == null) return;
            inputReader.NavigateEvent -= OnNavigate;
            inputReader.SubmitEvent -= Interact;
        }

        public void ChangePosition(int change)
        {
            currentPosition += change;
            if (change != 0) SoundManager.Instance?.PlaySound(changeSound);
            currentPosition = currentPosition < 0 ? buttons.Length - 1 : currentPosition >= buttons.Length ? 0 : currentPosition;
            arrow.position = new Vector3(arrow.position.x, buttons[currentPosition].position.y);
        }

        public void ContinueGame() => LoadingManager.LoadSpecificLevel(GameManager.Instance?.GetLastSavedLevelIndex() ?? 1);

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
                GameManager.Instance?.ResetCoins();
                GameManager.Instance?.SaveGame(true);
                LoadingManager.LoadSpecificLevel(1);
            }
            else if (currentPosition == 1) Application.Quit();
        }
    }
}
