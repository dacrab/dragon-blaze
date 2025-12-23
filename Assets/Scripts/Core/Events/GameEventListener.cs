using UnityEngine;
using UnityEngine.Events;

namespace Core.Events
{
    /// <summary>
    /// MonoBehaviour-based listener for ScriptableObject GameEvents.
    /// Makes it easy to wire up events in the Unity Inspector.
    /// More expandable than static EventBus for editor-driven workflows.
    /// </summary>
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent gameEvent;
        [SerializeField] private UnityEvent response;

        private void OnEnable()
        {
            if (gameEvent != null)
                gameEvent.Register(OnEventRaised);
        }

        private void OnDisable()
        {
            if (gameEvent != null)
                gameEvent.Unregister(OnEventRaised);
        }

        private void OnEventRaised() => response?.Invoke();
    }

    /// <summary>
    /// Generic version for events with a single parameter.
    /// </summary>
    public class GameEventListener<T> : MonoBehaviour
    {
        [SerializeField] private GameEvent<T> gameEvent;
        [SerializeField] private UnityEvent<T> response;

        private void OnEnable()
        {
            if (gameEvent != null)
                gameEvent.Register(OnEventRaised);
        }

        private void OnDisable()
        {
            if (gameEvent != null)
                gameEvent.Unregister(OnEventRaised);
        }

        private void OnEventRaised(T value) => response?.Invoke(value);
    }
}

