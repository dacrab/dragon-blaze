using UnityEngine;

namespace Core.Managers
{
    /// <summary>
    /// Base class for singleton managers with proper lifecycle management.
    /// </summary>
    public abstract class SingletonManager<T> : BaseManager where T : SingletonManager<T>
    {
        #region Singleton
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        Debug.LogWarning($"[{typeof(T).Name}] Instance not found. Creating temporary instance.");
                        var go = new GameObject(typeof(T).Name);
                        instance = go.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected virtual bool ShouldPersist => true;
        #endregion

        #region Unity Lifecycle
        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                if (ShouldPersist)
                {
                    DontDestroyOnLoad(gameObject);
                }
                base.Awake();
            }
            else if (instance != this)
            {
                Debug.LogWarning($"[{typeof(T).Name}] Duplicate instance detected. Destroying.");
                Destroy(gameObject);
            }
        }

        protected override void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            base.OnDestroy();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Checks if the singleton instance exists.
        /// </summary>
        protected static bool HasInstance => instance != null;
        #endregion
    }
}

