using System;
using UnityEngine;

namespace Core.Events
{
    /// <summary>
    /// Generic game event for type-safe event handling.
    /// </summary>
    [CreateAssetMenu(fileName = "GameEvent", menuName = "DragonBlaze/Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private event Action OnEventRaised;

        public void Raise()
        {
            OnEventRaised?.Invoke();
        }

        public void Register(Action handler)
        {
            OnEventRaised += handler;
        }

        public void Unregister(Action handler)
        {
            OnEventRaised -= handler;
        }
    }

    /// <summary>
    /// Generic game event with one parameter for type-safe event handling.
    /// </summary>
    public class GameEvent<T> : ScriptableObject
    {
        private event Action<T> OnEventRaised;

        public void Raise(T value)
        {
            OnEventRaised?.Invoke(value);
        }

        public void Register(Action<T> handler)
        {
            OnEventRaised += handler;
        }

        public void Unregister(Action<T> handler)
        {
            OnEventRaised -= handler;
        }
    }

    /// <summary>
    /// Generic game event with two parameters for type-safe event handling.
    /// </summary>
    public class GameEvent<T1, T2> : ScriptableObject
    {
        private event Action<T1, T2> OnEventRaised;

        public void Raise(T1 value1, T2 value2)
        {
            OnEventRaised?.Invoke(value1, value2);
        }

        public void Register(Action<T1, T2> handler)
        {
            OnEventRaised += handler;
        }

        public void Unregister(Action<T1, T2> handler)
        {
            OnEventRaised -= handler;
        }
    }
}

