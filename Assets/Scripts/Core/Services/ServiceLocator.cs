using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new();
        private static readonly Dictionary<Type, Func<object>> factories = new();

        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Service {type.Name} already registered. Replacing.");
            }
            services[type] = service;
        }

        public static void RegisterFactory<T>(Func<T> factory) where T : class
        {
            factories[typeof(T)] = () => factory();
        }

        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            
            if (services.TryGetValue(type, out var service))
                return (T)service;
            
            if (factories.TryGetValue(type, out var factory))
            {
                var instance = (T)factory();
                services[type] = instance;
                return instance;
            }
            
            Debug.LogWarning($"[ServiceLocator] Service {type.Name} not found.");
            return null;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            service = Get<T>();
            return service != null;
        }

        public static void Unregister<T>() where T : class
        {
            services.Remove(typeof(T));
        }

        public static void Clear()
        {
            services.Clear();
            factories.Clear();
        }
    }
}
