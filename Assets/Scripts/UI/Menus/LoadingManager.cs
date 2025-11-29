using UnityEngine;
using UnityEngine.SceneManagement;
using UI.Managers;
using Cysharp.Threading.Tasks;

namespace UI.Menus
{
    public class LoadingManager : MonoBehaviour
    {
        private static LoadingManager _instance;
        public static LoadingManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<LoadingManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject(nameof(LoadingManager));
                        _instance = go.AddComponent<LoadingManager>();
                    }
                }
                return _instance;
            }
        }

        [SerializeField] private UIManager uiManager;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public static void LoadNextLevel() =>
            Instance.LoadLevelAsync(SceneManager.GetActiveScene().buildIndex + 1).Forget();

        public static void LoadSpecificLevel(int levelIndex) =>
            Instance.LoadLevelAsync(levelIndex).Forget();

        private async UniTaskVoid LoadLevelAsync(int levelIndex)
        {
            EnsureUIManager();
            uiManager?.ShowLoadingScreen(true);

            var operation = SceneManager.LoadSceneAsync(levelIndex);
            if (operation == null)
            {
                Debug.LogError($"Scene index {levelIndex} could not be loaded.");
                uiManager?.ShowLoadingScreen(false);
                return;
            }

            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                uiManager?.UpdateLoadingImage(Mathf.Clamp01(operation.progress / 0.9f));
                await UniTask.Yield();
            }

            uiManager?.UpdateLoadingImage(1f);
            await UniTask.Delay(500);
            operation.allowSceneActivation = true;

            await UniTask.WaitUntil(() => operation.isDone);
            uiManager?.ShowLoadingScreen(false);
        }

        private void EnsureUIManager()
        {
            if (uiManager == null)
                uiManager = FindFirstObjectByType<UIManager>();
        }
    }
}
