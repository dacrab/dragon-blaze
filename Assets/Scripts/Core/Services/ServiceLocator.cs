using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services
{
    /// <summary>
    /// Lightweight service locator. Services register themselves in Awake and unregister in OnDestroy.
    /// Cleared on domain reload to avoid stale references.
    /// </summary>
    public static class ServiceLocator
    {
        static readonly Dictionary<Type, object> services = new();

        public static void Register<T>(T service) where T : class => services[typeof(T)] = service;

        public static void Unregister<T>() where T : class => services.Remove(typeof(T));

        public static T Get<T>() where T : class =>
            services.TryGetValue(typeof(T), out var service) ? (T)service : null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => services.Clear();
    }
}