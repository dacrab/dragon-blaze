using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Core.Managers;
using Core.Constants;
using Core.Input;

namespace UI.Menus
{
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

        [Header("Volume Display")]
        [SerializeField] TextMeshProUGUI musicVolumeText, soundVolumeText;

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
            UpdateArrow();
            UpdateVolumeDisplays();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        void OnEnable()
        {
            inputReader.NavigateEvent += OnNavigate;
            inputReader.SubmitEvent += OnSubmit;
            inputReader.EnableUIInput();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMusicVolumeChanged += UpdateMusicVolume;
                GameManager.Instance.OnSoundVolumeChanged += UpdateSoundVolume;
            }
        }

        void OnDisable()
        {
            inputReader.NavigateEvent -= OnNavigate;
            inputReader.SubmitEvent -= OnSubmit;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMusicVolumeChanged -= UpdateMusicVolume;
                GameManager.Instance.OnSoundVolumeChanged -= UpdateSoundVolume;
            }
        }

        void OnNavigate(Vector2 dir)
        {
            float threshold = gameConfig.navigationThreshold;
            if (dir.y > threshold) ChangeIndex(-1);
            else if (dir.y < -threshold) ChangeIndex(1);
        }

        void ChangeIndex(int delta)
        {
            currentIndex = (currentIndex + delta + buttons.Length) % buttons.Length;
            GameManager.Instance?.PlaySound(changeSound);
            UpdateArrow();
        }

        void UpdateArrow() => arrow.position = new(arrow.position.x, buttons[currentIndex].position.y, arrow.position.z);

        void UpdateVolumeDisplays()
        {
            if (GameManager.Instance == null) return;
            UpdateMusicVolume(GameManager.Instance.MusicVolume);
            UpdateSoundVolume(GameManager.Instance.SoundVolume);
        }

        void UpdateMusicVolume(float value) => musicVolumeText.text = $"{value * 100:F0}";
        void UpdateSoundVolume(float value) => soundVolumeText.text = $"{value * 100:F0}";

        void OnSubmit()
        {
            GameManager.Instance?.PlaySound(interactSound);
            menuActions[currentIndex].action?.Invoke();
        }

        public void StartNewGame()
        {
            GameManager.Instance?.ResetCoins();
            GameManager.Instance?.SaveGame(true);
            LoadingManager.LoadSpecificLevel(gameConfig.firstLevelSceneIndex);
        }

        public void QuitGame() => Application.Quit();
    }
}
