using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Core.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "DragonBlaze/Input/Input Reader")]
    public class InputReader : ScriptableObject
    {
        [SerializeField] private InputActionAsset inputActions;

        private InputAction _moveAction, _jumpAction, _dashAction, _attackAction;
        private InputAction _interactAction, _pauseAction, _navigateAction, _submitAction;

        public event UnityAction<float> MoveEvent;
        public event UnityAction JumpEvent;
        public event UnityAction JumpCanceledEvent;
        public event UnityAction DashEvent;
        public event UnityAction AttackEvent;
        public event UnityAction InteractEvent;
        public event UnityAction PauseEvent;
        public event UnityAction<Vector2> NavigateEvent;
        public event UnityAction SubmitEvent;

        private void OnEnable()
        {
            if (inputActions == null) return;
            CacheActions();
            SubscribeToActions();
            EnableGameplayInput();
        }

        private void OnDisable() => UnsubscribeFromActions();

        private void CacheActions()
        {
            _moveAction = inputActions.FindAction("Gameplay/Move");
            _jumpAction = inputActions.FindAction("Gameplay/Jump");
            _dashAction = inputActions.FindAction("Gameplay/Dash");
            _attackAction = inputActions.FindAction("Gameplay/Attack");
            _interactAction = inputActions.FindAction("Gameplay/Interact");
            _pauseAction = inputActions.FindAction("Gameplay/Pause");
            _navigateAction = inputActions.FindAction("UI/Navigate");
            _submitAction = inputActions.FindAction("UI/Submit");
        }

        private void SubscribeToActions()
        {
            if (_moveAction != null) { _moveAction.performed += ctx => MoveEvent?.Invoke(ctx.ReadValue<float>()); _moveAction.canceled += ctx => MoveEvent?.Invoke(0); }
            if (_jumpAction != null) { _jumpAction.performed += _ => JumpEvent?.Invoke(); _jumpAction.canceled += _ => JumpCanceledEvent?.Invoke(); }
            if (_dashAction != null) _dashAction.performed += _ => DashEvent?.Invoke();
            if (_attackAction != null) _attackAction.performed += _ => AttackEvent?.Invoke();
            if (_interactAction != null) _interactAction.performed += _ => InteractEvent?.Invoke();
            if (_pauseAction != null) _pauseAction.performed += _ => PauseEvent?.Invoke();
            if (_navigateAction != null) _navigateAction.performed += ctx => NavigateEvent?.Invoke(ctx.ReadValue<Vector2>());
            if (_submitAction != null) _submitAction.performed += _ => SubmitEvent?.Invoke();
        }

        private void UnsubscribeFromActions()
        {
            _moveAction?.Disable();
            _jumpAction?.Disable();
            _dashAction?.Disable();
            _attackAction?.Disable();
            _interactAction?.Disable();
            _pauseAction?.Disable();
            _navigateAction?.Disable();
            _submitAction?.Disable();
        }

        // Manual raise methods for InputProvider fallback
        public void RaiseMoveEvent(float value) => MoveEvent?.Invoke(value);
        public void RaiseJumpEvent() => JumpEvent?.Invoke();
        public void RaiseJumpCanceledEvent() => JumpCanceledEvent?.Invoke();
        public void RaiseDashEvent() => DashEvent?.Invoke();
        public void RaiseAttackEvent() => AttackEvent?.Invoke();
        public void RaiseInteractEvent() => InteractEvent?.Invoke();
        public void RaisePauseEvent() => PauseEvent?.Invoke();

        public void EnableGameplayInput()
        {
            inputActions?.FindActionMap("UI")?.Disable();
            inputActions?.FindActionMap("Gameplay")?.Enable();
        }

        public void EnableUIInput()
        {
            inputActions?.FindActionMap("Gameplay")?.Disable();
            inputActions?.FindActionMap("UI")?.Enable();
        }
    }
}
