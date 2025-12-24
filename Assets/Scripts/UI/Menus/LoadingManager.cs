using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Services;
using Core.Utilities;
using UI.Managers;

namespace UI.Menus
{
    public class LoadingManager : MonoBehaviour
    {
        #region Singleton
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
                        GameObject obj = new GameObject();
                        obj.name = typeof(LoadingManager).Name;
                        instance = obj.AddComponent<LoadingManager>();
                    }
                }
                return instance;
            }
        }

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
            }
            
            AutoWireHelper.WireAllFields(this);
        }
        #endregion

        #region Serialized Fields
        [Header("Scene Loading Settings")]
        [AutoWire(AutoWireAttribute.WireType.Scene)]
        [SerializeField] private UIManager uiManager;
        #endregion

        #region Public Methods
        public static void LoadNextLevel()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            Instance.StartCoroutine(Instance.LoadLevel(nextSceneIndex));
        }

        public static void LoadSpecificLevel(int levelIndex)
        {
            Instance.StartCoroutine(Instance.LoadLevel(levelIndex));
        }
        #endregion

        #region Private Methods
        private IEnumerator LoadLevel(int levelIndex)
        {
            if (uiManager) uiManager.ShowLoadingScreen(true);

            AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
            if (operation == null)
            {
                Debug.LogError($"Scene index {levelIndex} could not be loaded.");
                if (uiManager) uiManager.ShowLoadingScreen(false);
                yield break;
            }
            
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                if (uiManager) uiManager.UpdateLoadingImage(progress);

                if (operation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(0.5f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            if (uiManager) uiManager.ShowLoadingScreen(false);
        }

        #endregion
    }
}
