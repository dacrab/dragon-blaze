namespace Core.Interfaces
{
    /// <summary>
    /// Interface for objects that can be pooled using Unity's built-in pooling system.
    /// Implement this interface to get automatic lifecycle callbacks.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called when the object is retrieved from the pool (OnGet).
        /// Use this to reset state, re-enable components, etc.
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// Called when the object is returned to the pool (OnRelease).
        /// Use this to clean up, disable components, etc.
        /// </summary>
        void OnDespawn();
    }
}
