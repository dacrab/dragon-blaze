using UnityEngine;
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
        public event UnityAction ResumeEvent;
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
        public void RaiseResumeEvent() => ResumeEvent?.Invoke();
        public void EnableGameplayInput() { }
        public void EnableUIInput() { }
        public void DisableAllInput() { }
    }
}
