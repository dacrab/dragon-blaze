using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Core.Input
{
    /// <summary>
    /// ScriptableObject-based input reader using Unity's Input System.
    /// Assign the InputActionAsset in the Inspector for rebinding support.
    /// </summary>
    [CreateAssetMenu(fileName = "InputReader", menuName = "DragonBlaze/Input/Input Reader")]
    public class InputReader : ScriptableObject
    {
        [Header("Input Actions Asset (Optional)")]
        [Tooltip("Assign an InputActionAsset for full Input System support. If null, use InputProvider for legacy input.")]
        [SerializeField] private InputActionAsset inputActions;

        // Cached action references
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _dashAction;
        private InputAction _attackAction;
        private InputAction _interactAction;
        private InputAction _pauseAction;
        private InputAction _navigateAction;
        private InputAction _submitAction;
        private InputAction _cancelAction;

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

        public bool HasInputActions => inputActions != null;

        private void OnEnable()
        {
            if (inputActions == null) return;

            CacheActions();
            SubscribeToActions();
            EnableGameplayInput();
        }

        private void OnDisable()
        {
            UnsubscribeFromActions();
        }

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
            _cancelAction = inputActions.FindAction("UI/Cancel");
        }

        private void SubscribeToActions()
        {
            if (_moveAction != null)
            {
                _moveAction.performed += OnMove;
                _moveAction.canceled += OnMove;
            }
            if (_jumpAction != null)
            {
                _jumpAction.performed += OnJump;
                _jumpAction.canceled += OnJumpCanceled;
            }
            if (_dashAction != null) _dashAction.performed += OnDash;
            if (_attackAction != null) _attackAction.performed += OnAttack;
            if (_interactAction != null) _interactAction.performed += OnInteract;
            if (_pauseAction != null) _pauseAction.performed += OnPause;
            if (_navigateAction != null) _navigateAction.performed += OnNavigate;
            if (_submitAction != null) _submitAction.performed += OnSubmit;
            if (_cancelAction != null) _cancelAction.performed += OnCancel;
        }

        private void UnsubscribeFromActions()
        {
            if (_moveAction != null)
            {
                _moveAction.performed -= OnMove;
                _moveAction.canceled -= OnMove;
            }
            if (_jumpAction != null)
            {
                _jumpAction.performed -= OnJump;
                _jumpAction.canceled -= OnJumpCanceled;
            }
            if (_dashAction != null) _dashAction.performed -= OnDash;
            if (_attackAction != null) _attackAction.performed -= OnAttack;
            if (_interactAction != null) _interactAction.performed -= OnInteract;
            if (_pauseAction != null) _pauseAction.performed -= OnPause;
            if (_navigateAction != null) _navigateAction.performed -= OnNavigate;
            if (_submitAction != null) _submitAction.performed -= OnSubmit;
            if (_cancelAction != null) _cancelAction.performed -= OnCancel;
        }

        #region Input Callbacks
        private void OnMove(InputAction.CallbackContext ctx) => MoveEvent?.Invoke(ctx.ReadValue<float>());
        private void OnJump(InputAction.CallbackContext ctx) => JumpEvent?.Invoke();
        private void OnJumpCanceled(InputAction.CallbackContext ctx) => JumpCanceledEvent?.Invoke();
        private void OnDash(InputAction.CallbackContext ctx) => DashEvent?.Invoke();
        private void OnAttack(InputAction.CallbackContext ctx) => AttackEvent?.Invoke();
        private void OnInteract(InputAction.CallbackContext ctx) => InteractEvent?.Invoke();
        private void OnPause(InputAction.CallbackContext ctx) => PauseEvent?.Invoke();
        private void OnNavigate(InputAction.CallbackContext ctx) => NavigateEvent?.Invoke(ctx.ReadValue<Vector2>());
        private void OnSubmit(InputAction.CallbackContext ctx) => SubmitEvent?.Invoke();
        private void OnCancel(InputAction.CallbackContext ctx) => CancelEvent?.Invoke();
        #endregion

        #region Public Methods for Manual Raising (used by InputProvider)
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
        #endregion

        #region Input Map Switching
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

        public void DisableAllInput() => inputActions?.Disable();
        public void EnableAllInput() => inputActions?.Enable();
        #endregion

        #region Rebinding Support
        public string GetBindingDisplayString(string actionName, int bindingIndex = 0)
        {
            var action = inputActions?.FindAction(actionName);
            return action?.GetBindingDisplayString(bindingIndex) ?? string.Empty;
        }

        public InputActionRebindingExtensions.RebindingOperation StartRebind(string actionName, int bindingIndex, System.Action onComplete = null)
        {
            var action = inputActions?.FindAction(actionName);
            if (action == null) return null;

            action.Disable();
            return action.PerformInteractiveRebinding(bindingIndex)
                .OnComplete(operation =>
                {
                    action.Enable();
                    onComplete?.Invoke();
                    operation.Dispose();
                })
                .Start();
        }
        #endregion
    }
}
