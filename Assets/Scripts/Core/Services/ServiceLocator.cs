using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Interfaces;

namespace Core.Services
{
    /// <summary>
    /// Service locator pattern implementation for dependency injection.
    /// Provides centralized access to game services with lifecycle management.
    /// </summary>
    public static class ServiceLocator
    {
        #region Private Fields
        private static readonly Dictionary<Type, object> services = new();
        private static readonly Dictionary<Type, Func<object>> factories = new();
        #endregion

        #region Registration
        /// <summary>
        /// Registers a service instance. If service implements IService, Initialize() is called.
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Service {type.Name} already registered. Replacing.");
                Unregister<T>();
            }
            
            services[type] = service;
            
            if (service is IService serviceInterface)
            {
                serviceInterface.Initialize();
            }
        }

        /// <summary>
        /// Registers a factory function for lazy service creation.
        /// </summary>
        public static void RegisterFactory<T>(Func<T> factory) where T : class
        {
            factories[typeof(T)] = () => factory();
        }
        #endregion

        #region Retrieval
        /// <summary>
        /// Gets a service instance. Returns null if not found.
        /// </summary>
        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            
            if (services.TryGetValue(type, out var service))
                return (T)service;
            
            if (factories.TryGetValue(type, out var factory))
            {
                var instance = (T)factory();
                Register(instance);
                return instance;
            }
            
            Debug.LogWarning($"[ServiceLocator] Service {type.Name} not found.");
            return null;
        }

        /// <summary>
        /// Attempts to get a service instance. Returns true if found, false otherwise.
        /// </summary>
        public static bool TryGet<T>(out T service) where T : class
        {
            service = Get<T>();
            return service != null;
        }

        /// <summary>
        /// Checks if a service is registered.
        /// </summary>
        public static bool IsRegistered<T>() where T : class
        {
            return services.ContainsKey(typeof(T)) || factories.ContainsKey(typeof(T));
        }
        #endregion

        #region Unregistration
        /// <summary>
        /// Unregisters a service. If service implements IService, Shutdown() is called.
        /// </summary>
        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                if (service is IService serviceInterface)
                {
                    serviceInterface.Shutdown();
                }
                services.Remove(type);
            }
            factories.Remove(type);
        }

        /// <summary>
        /// Clears all registered services and factories.
        /// </summary>
        public static void Clear()
        {
            // Shutdown all services that implement IService
            foreach (var service in services.Values)
            {
                if (service is IService serviceInterface)
                {
                    serviceInterface.Shutdown();
                }
            }
            
            services.Clear();
            factories.Clear();
        }
        #endregion

        #region Debug
        /// <summary>
        /// Logs all registered services (for debugging).
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogRegisteredServices()
        {
            Debug.Log($"[ServiceLocator] Registered Services: {services.Count}");
            foreach (var kvp in services)
            {
                Debug.Log($"  - {kvp.Key.Name}");
            }
        }
        #endregion
    }
}
