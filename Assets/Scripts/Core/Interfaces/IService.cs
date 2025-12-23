namespace Core.Interfaces
{
    /// <summary>
    /// Base interface for all services in the ServiceLocator pattern.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Called when the service is registered.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called when the service is unregistered or the game shuts down.
        /// </summary>
        void Shutdown();
    }
}

