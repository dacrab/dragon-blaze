using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Core.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "DragonBlaze/Input/Input Reader")]
    public class InputReader : ScriptableObject
    {
        // Gameplay Events
        public event UnityAction<float> MoveEvent;
        public event UnityAction JumpEvent;
        public event UnityAction JumpCanceledEvent;
        public event UnityAction DashEvent;
        public event UnityAction AttackEvent;
        public event UnityAction InteractEvent;
        public event UnityAction PauseEvent;

        // UI Events
        public event UnityAction<Vector2> NavigateEvent;
        public event UnityAction SubmitEvent;
        public event UnityAction CancelEvent;
        // public event UnityAction ResumeEvent; // Currently unused

        // Manual Input System Polling (since we don't have the generated class)
        // Ideally, this would use InputActionAsset, but for code-only solution:
        
        private void OnEnable()
        {
            // If we had the generated class, we would initialize it here.
            // Without it, we rely on polling in Update via a MonoBehaviour runner or similar.
            // However, ScriptableObjects don't Update.
            // For this refactor, we will create a simple runtime poller.
        }

        // For compatibility with existing code expecting event invocation
        // We will rely on a Monobehaviour "InputProvider" in the scene to drive this
        public void RaiseMoveEvent(float value) => MoveEvent?.Invoke(value);
        public void RaiseJumpEvent() => JumpEvent?.Invoke();
        public void RaiseJumpCanceledEvent() => JumpCanceledEvent?.Invoke();
        public void RaiseDashEvent() => DashEvent?.Invoke();
        public void RaiseAttackEvent() => AttackEvent?.Invoke();
        public void RaiseInteractEvent() => InteractEvent?.Invoke();
        public void RaisePauseEvent() => PauseEvent?.Invoke();
        
        public void RaiseNavigateEvent(Vector2 value) => NavigateEvent?.Invoke(value);
        public void RaiseSubmitEvent() => SubmitEvent?.Invoke();
        public void RaiseCancelEvent() => CancelEvent?.Invoke();

        public void EnableGameplayInput() { } // Placeholder
        public void EnableUIInput() { } // Placeholder
        public void DisableAllInput() { } // Placeholder
    }
}
