using System;
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

        static LoadingManager? instance;
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
        void ISceneLoader.LoadScene(string? sceneName) => _ = LoadAsync(sceneName);

        void LoadNextLevel()
        {
            string active = SceneManager.GetActiveScene().name;
            // The last scene in levelOrder (the credits roll) has no successor and navigates itself.
            if (GameConfig.Default.TryGetNextLevel(active, out var next)) _ = LoadAsync(next);
        }

        async Awaitable LoadAsync(string? sceneName)
        {
            if (isLoading) return;
            isLoading = true;
            var ui = ServiceLocator.Get<UIManager>();
            try
            {
                ui?.ShowLoadingScreen(true);

                if (string.IsNullOrEmpty(sceneName))
                {
                    Debug.LogWarning("[LoadingManager] No scene name provided; falling back to first level.");
                    sceneName = GameConfig.Default.FirstLevelSceneName;
                }
                var op = string.IsNullOrEmpty(sceneName) ? null : SceneManager.LoadSceneAsync(sceneName);
                if (op == null)
                {
                    Debug.LogWarning($"[LoadingManager] Scene '{sceneName}' is not in Build Settings; falling back to first level.");
                    var fallback = GameConfig.Default.FirstLevelSceneName;
                    if (string.IsNullOrEmpty(fallback)) return;
                    op = SceneManager.LoadSceneAsync(fallback);
                    if (op == null) return;
                }
                // Scenes in Build Settings cap async progress at 0.9 until allowSceneActivation, so that is "done".
                op.allowSceneActivation = false;

                // Guard a misconfigured threshold: 0 would divide by zero, >=1 would never be reached.
                float threshold = Mathf.Clamp(loadingProgressThreshold, 0.01f, 0.9f);
                while (op.progress < threshold)
                {
                    ui?.UpdateLoadingImage(op.progress / threshold);
                    await Awaitable.NextFrameAsync();
                }

                ui?.UpdateLoadingImage(1f);
                // Unscaled: a load requested from the game-over or menu state may run with timeScale 0.
                float end = Time.unscaledTime + minimumLoadingTime;
                while (Time.unscaledTime < end) await Awaitable.NextFrameAsync();
                op.allowSceneActivation = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[LoadingManager] Load of '{sceneName}' failed: {e.Message}");
            }
            finally
            {
                // Always drop the overlay and unlock the loader, or the game is stuck behind a loading screen.
                ui?.ShowLoadingScreen(false);
                isLoading = false;
            }
        }
    }
}
