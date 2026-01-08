using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI.Managers;

namespace UI.Menus
{

public sealed class LoadingManager : MonoBehaviour
{
    [Header("Loading Settings")]
    [SerializeField] float loadingProgressThreshold = 0.9f;
    [SerializeField] float minimumLoadingTime = 0.3f;

    static LoadingManager instance;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else if (instance != this) Destroy(gameObject);
    }

    public static void LoadNextLevel() => LoadSpecificLevel(SceneManager.GetActiveScene().buildIndex + 1);

    public static void LoadSpecificLevel(int level)
    {
        if (instance == null)
        {
            var obj = new GameObject(nameof(LoadingManager));
            instance = obj.AddComponent<LoadingManager>();
            DontDestroyOnLoad(obj);
        }
        instance.StartCoroutine(instance.LoadAsync(level));
    }

    IEnumerator LoadAsync(int level)
    {
        var ui = FindFirstObjectByType<UIManager>();
        ui?.ShowLoadingScreen(true);

        var op = SceneManager.LoadSceneAsync(level);
        if (op == null) { ui?.ShowLoadingScreen(false); yield break; }
        op.allowSceneActivation = false;

        while (op.progress < loadingProgressThreshold)
        {
            ui?.UpdateLoadingImage(op.progress / loadingProgressThreshold);
            yield return null;
        }

        ui?.UpdateLoadingImage(1f);
        yield return new WaitForSeconds(minimumLoadingTime);
        op.allowSceneActivation = true;
        ui?.ShowLoadingScreen(false);
    }
}
}