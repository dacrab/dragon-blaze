using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                instance = FindObjectOfType<LoadingManager>();
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
    }
    #endregion

    #region Serialized Fields
    [Header("Scene Loading Settings")]
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
        EnsureUIManager();

        uiManager.ShowLoadingScreen(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            uiManager.UpdateLoadingImage(progress);

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f); // Short delay for visual feedback
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        uiManager.ShowLoadingScreen(false);
    }

    private void EnsureUIManager()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogWarning("UIManager not found in the scene. Creating a temporary one.");
                GameObject tempUIManager = new GameObject("Temporary UIManager");
                uiManager = tempUIManager.AddComponent<UIManager>();
            }
        }
    }
    #endregion
}