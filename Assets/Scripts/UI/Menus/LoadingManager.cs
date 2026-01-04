using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI.Managers;

namespace UI.Menus
{
    public class LoadingManager : MonoBehaviour
    {
        private static LoadingManager instance;
        public static LoadingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<LoadingManager>();
                    if (instance == null)
                    {
                        var obj = new GameObject(nameof(LoadingManager));
                        instance = obj.AddComponent<LoadingManager>();
                    }
                }
                return instance;
            }
        }

        private UIManager uiManager;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            uiManager = FindFirstObjectByType<UIManager>();
        }

        public static void LoadNextLevel()
        {
            int next = SceneManager.GetActiveScene().buildIndex + 1;
            Instance.StartCoroutine(Instance.LoadLevel(next));
        }

        public static void LoadSpecificLevel(int levelIndex)
        {
            Instance.StartCoroutine(Instance.LoadLevel(levelIndex));
        }

        private IEnumerator LoadLevel(int levelIndex)
        {
            if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();
            uiManager?.ShowLoadingScreen(true);

            var op = SceneManager.LoadSceneAsync(levelIndex);
            if (op == null)
            {
                uiManager?.ShowLoadingScreen(false);
                yield break;
            }
            
            op.allowSceneActivation = false;

            while (!op.isDone)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                uiManager?.UpdateLoadingImage(progress);

                if (op.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(0.5f);
                    op.allowSceneActivation = true;
                }
                yield return null;
            }

            uiManager?.ShowLoadingScreen(false);
        }
    }
}
