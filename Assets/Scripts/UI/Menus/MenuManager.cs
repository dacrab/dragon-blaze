using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Core.Constants;
using Core.Input;
using Core.Managers;
using Core.Services;

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

        void Awake()
        {
            UpdateArrow();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
            currentIndex = (currentIndex + delta + buttons.Length) % buttons.Length;
            ServiceLocator.Get<IAudioManager>()?.PlaySound(changeSound);
            UpdateArrow();
        }

        void UpdateArrow() => arrow.position = new(arrow.position.x, buttons[currentIndex].position.y, arrow.position.z);

        void UpdateMusicVolume(float value) => musicVolumeText.text = $"{value * 100:F0}";
        void UpdateSoundVolume(float value) => soundVolumeText.text = $"{value * 100:F0}";

        void OnSubmit()
        {
            ServiceLocator.Get<IAudioManager>()?.PlaySound(interactSound);
            menuActions[currentIndex].action?.Invoke();
        }

        public void StartNewGame()
        {
            ServiceLocator.Get<IGameManager>()?.ResetCoins();
            ServiceLocator.Get<IGameManager>()?.SaveGame(true);
            ServiceLocator.Get<ISceneLoader>()?.LoadScene(GameConfig.Default.FirstLevelSceneName);
        }

        public void QuitGame() => Application.Quit();
    }
}
