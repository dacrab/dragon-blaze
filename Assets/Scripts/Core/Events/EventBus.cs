using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Events
{
    /// <summary>
    /// Type-keyed event bus. Raise or subscribe with a payload struct from GameEvents.
    /// Adding a new event only requires a new payload struct, never editing this class.
    /// </summary>
    public static class EventBus
    {
        static readonly Dictionary<Type, Delegate> handlers = new();

        public static void Subscribe<T>(Action<T> handler) where T : struct =>
            handlers[typeof(T)] = (Action<T>)handlers.GetValueOrDefault(typeof(T)) + handler;

        public static void Unsubscribe<T>(Action<T> handler) where T : struct =>
            handlers[typeof(T)] = (Action<T>)handlers.GetValueOrDefault(typeof(T)) - handler;

        public static void Raise<T>(T payload) where T : struct =>
            ((Action<T>)handlers.GetValueOrDefault(typeof(T)))?.Invoke(payload);

        internal static void Clear() => handlers.Clear();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => handlers.Clear();
    }
}
