using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Events;
using UI.Managers;

namespace UI.Menus
{
    public sealed class LoadingManager : MonoBehaviour
    {
        [SerializeField] float loadingProgressThreshold = 0.9f;
        [SerializeField] float minimumLoadingTime = 0.3f;

        static LoadingManager instance;

        void Awake()
        {
            if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
            else if (instance != this) Destroy(gameObject);
        }

        void OnEnable() => EventBus.OnRequestNextLevel += LoadNextLevel;
        void OnDisable() => EventBus.OnRequestNextLevel -= LoadNextLevel;

        public static void LoadNextLevel() => LoadSpecificLevel(SceneManager.GetActiveScene().buildIndex + 1);

        public static void LoadSpecificLevel(int level)
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(LoadingManager));
                instance = obj.AddComponent<LoadingManager>();
                DontDestroyOnLoad(obj);
            }
            instance.LoadAsync(level);
        }

        async void LoadAsync(int level)
        {
            var ui = FindFirstObjectByType<UIManager>();
            ui?.ShowLoadingScreen(true);

            var op = SceneManager.LoadSceneAsync(level);
            if (op == null) { ui?.ShowLoadingScreen(false); return; }
            op.allowSceneActivation = false;

            while (op.progress < loadingProgressThreshold)
            {
                ui?.UpdateLoadingImage(op.progress / loadingProgressThreshold);
                await Awaitable.NextFrameAsync();
            }

            ui?.UpdateLoadingImage(1f);
            await Awaitable.WaitForSecondsAsync(minimumLoadingTime);
            op.allowSceneActivation = true;
            ui?.ShowLoadingScreen(false);
        }
    }
}
