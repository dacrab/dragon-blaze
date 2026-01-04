using UnityEngine;

namespace Core.Managers
{
    public abstract class SingletonManager<T> : MonoBehaviour where T : SingletonManager<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                        Debug.LogWarning($"[{typeof(T).Name}] Instance not found.");
                }
                return instance;
            }
        }

        public bool IsInitialized { get; private set; }
        protected virtual bool ShouldPersist => true;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                if (ShouldPersist) DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
                Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                OnShutdown();
                instance = null;
            }
        }

        private void Initialize()
        {
            if (IsInitialized) return;
            OnInitialize();
            IsInitialized = true;
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnShutdown() { }
        protected static bool HasInstance => instance != null;
    }
}
