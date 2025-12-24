using UnityEngine;
using Core.Interfaces;

namespace Core.Managers
{
    /// <summary>
    /// Base class for all managers with common functionality.
    /// </summary>
    public abstract class BaseManager : MonoBehaviour, IService
    {
        #region Properties
        public bool IsInitialized { get; protected set; }
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            Shutdown();
        }
        #endregion

        #region IService Implementation
        public virtual void Initialize()
        {
            if (IsInitialized) return;
            OnInitialize();
            IsInitialized = true;
        }

        public virtual void Shutdown()
        {
            if (!IsInitialized) return;
            OnShutdown();
            IsInitialized = false;
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Override this method to implement custom initialization logic.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Override this method to implement custom shutdown logic.
        /// </summary>
        protected virtual void OnShutdown() { }
        #endregion
    }
}

