using UnityEngine;

namespace Core.Managers;

public abstract class SingletonManager<T> : MonoBehaviour where T : SingletonManager<T>
{
    public static T Instance { get; private set; }
    protected virtual bool Persist => true;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            if (Persist) DontDestroyOnLoad(gameObject);
            OnInit();
        }
        else if (Instance != this) Destroy(gameObject);
    }

    protected virtual void OnDestroy() { if (Instance == this) Instance = null; }
    protected virtual void OnInit() { }
}
