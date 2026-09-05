using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Core.Constants;
using Core.Input;
using Core.Managers;
using Core.Services;
using UI.Managers;

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

        [Header("Menu Actions")]
        [SerializeField] MenuAction[] menuActions;

        int currentIndex;
        InputReader inputReader;
        bool menuValid;

        void Awake()
        {
            menuValid = buttons is { Length: > 0 } && menuActions is { Length: > 0 } &&
                        menuActions.Length == buttons.Length;
            if (!menuValid)
            {
                Debug.LogWarning($"[{nameof(MenuManager)}] 'buttons'/'menuActions' counts mismatch " +
                                 $"({buttons?.Length ?? 0} vs {menuActions?.Length ?? 0}); menu navigation disabled.", this);
                return;
            }
            UpdateArrow();
        }

        void OnEnable()
        {
            inputReader = InputReader.Instance;
            if (inputReader != null)
            {
                inputReader.NavigateEvent += OnNavigate;
                inputReader.SubmitEvent += OnSubmit;
                inputReader.EnableUIInput();
            }
            if (ServiceLocator.Get<IAudioManager>() is { } audioManager)
            {
                audioManager.OnMusicVolumeChanged += UpdateMusicVolume;
                audioManager.OnSoundVolumeChanged += UpdateSoundVolume;
                UpdateMusicVolume(audioManager.MusicVolume);
                UpdateSoundVolume(audioManager.SoundVolume);
            }
        }

        void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.NavigateEvent -= OnNavigate;
                inputReader.SubmitEvent -= OnSubmit;
            }
            var audioManager = ServiceLocator.Get<IAudioManager>();
            if (audioManager != null)
            {
                audioManager.OnMusicVolumeChanged -= UpdateMusicVolume;
                audioManager.OnSoundVolumeChanged -= UpdateSoundVolume;
            }
        }

        void OnNavigate(Vector2 dir)
        {
            float threshold = GameConfig.Default.navigationThreshold;
            if (dir.y > threshold) ChangeIndex(-1);
            else if (dir.y < -threshold) ChangeIndex(1);
        }

        void ChangeIndex(int delta)
        {
            if (!menuValid) return;
            currentIndex = (currentIndex + delta + buttons.Length) % buttons.Length;
            ServiceLocator.Get<IAudioManager>()?.PlaySound(changeSound);
            UpdateArrow();
        }

        void UpdateArrow() => arrow.position = new(arrow.position.x, buttons[currentIndex].position.y, arrow.position.z);

        void UpdateMusicVolume(float value) => musicVolumeText.text = $"{value * 100:F0}";
        void UpdateSoundVolume(float value) => soundVolumeText.text = $"{value * 100:F0}";

        void OnSubmit()
        {
            if (!menuValid) return;
            ServiceLocator.Get<IAudioManager>()?.PlaySound(interactSound);
            menuActions[currentIndex].action?.Invoke();
        }

        public void StartNewGame() => UIManager.StartNewGame();

        public void QuitGame() => Application.Quit();
    }
}
