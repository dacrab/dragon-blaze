using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Events;
using Core.Services;
using UI.Managers;

namespace UI.Menus
{
    public sealed class LoadingManager : MonoBehaviour, ISceneLoader
    {
        [SerializeField] float loadingProgressThreshold = 0.9f;
        [SerializeField] float minimumLoadingTime = 0.3f;

        static LoadingManager instance;
        bool isLoading;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            if (instance != null) return;
            _ = new GameObject(nameof(LoadingManager)).AddComponent<LoadingManager>();
        }

        void Awake()
        {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<ISceneLoader>(this);
        }

        void OnDestroy()
        {
            if (instance != this) return;
            instance = null;
            ServiceLocator.Unregister<ISceneLoader>();
        }

        void OnEnable() => EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
        void OnDisable() => EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);

        void OnLevelCompleted(LevelCompletedEvent _) => LoadNextLevel();

        void ISceneLoader.LoadNextLevel() => LoadNextLevel();
        void ISceneLoader.LoadScene(string sceneName) => _ = LoadAsync(sceneName);

        void LoadNextLevel()
        {
            string active = SceneManager.GetActiveScene().name;
            if (GameConfig.Default.TryGetNextLevel(active, out var next)) _ = LoadAsync(next);
            else Debug.LogWarning($"[LoadingManager] No next level configured after '{active}'.");
        }

        async Awaitable LoadAsync(string sceneName)
        {
            if (isLoading) return;
            isLoading = true;
            try
            {
                var ui = FindFirstObjectByType<UIManager>();
                ui?.ShowLoadingScreen(true);

                var op = SceneManager.LoadSceneAsync(sceneName);
                if (op == null)
                {
                    Debug.LogWarning($"[LoadingManager] Scene '{sceneName}' is not in Build Settings.");
                    ui?.ShowLoadingScreen(false);
                    return;
                }
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
            finally { isLoading = false; }
        }
    }
}
