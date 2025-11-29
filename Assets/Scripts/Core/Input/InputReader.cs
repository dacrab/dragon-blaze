using System;
using UnityEngine;

namespace Core.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "DragonBlaze/Input/Input Reader")]
    public class InputReader : ScriptableObject
    {
        public event Action<float> MoveEvent;
        public event Action JumpEvent;
        public event Action JumpCanceledEvent;
        public event Action DashEvent;
        public event Action AttackEvent;
        public event Action InteractEvent;
        public event Action PauseEvent;

        public event Action<Vector2> NavigateEvent;
        public event Action SubmitEvent;
        public event Action CancelEvent;
        public event Action ResumeEvent;

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
    }
}
