using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Core.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "DragonBlaze/Input/Input Reader")]
    public sealed class InputReader : ScriptableObject
    {
        [SerializeField] InputActionAsset inputActions;

        public event UnityAction<float> MoveEvent;
        public event UnityAction JumpEvent, JumpCanceledEvent, DashEvent, AttackEvent, InteractEvent, PauseEvent, SubmitEvent;
        public event UnityAction<Vector2> NavigateEvent;

        InputActionMap gameplayMap, uiMap;

        void OnEnable()
        {
            if (inputActions == null) return;
            gameplayMap = inputActions.FindActionMap("Gameplay");
            uiMap = inputActions.FindActionMap("UI");

            if (gameplayMap != null)
            {
                gameplayMap.Enable();
                gameplayMap["Move"].performed += OnMove;
                gameplayMap["Move"].canceled += OnMoveCanceled;
                gameplayMap["Jump"].performed += OnJump;
                gameplayMap["Jump"].canceled += OnJumpCanceled;
                gameplayMap["Dash"].performed += OnDash;
                gameplayMap["Attack"].performed += OnAttack;
                gameplayMap["Interact"].performed += OnInteract;
                gameplayMap["Pause"].performed += OnPause;
            }

            if (uiMap != null)
            {
                uiMap["Navigate"].performed += OnNavigate;
                uiMap["Submit"].performed += OnSubmit;
            }
        }

        void OnDisable()
        {
            if (inputActions == null) return;
            inputActions.Disable();
            if (gameplayMap != null)
            {
                gameplayMap["Move"].performed -= OnMove;
                gameplayMap["Move"].canceled -= OnMoveCanceled;
                gameplayMap["Jump"].performed -= OnJump;
                gameplayMap["Jump"].canceled -= OnJumpCanceled;
                gameplayMap["Dash"].performed -= OnDash;
                gameplayMap["Attack"].performed -= OnAttack;
                gameplayMap["Interact"].performed -= OnInteract;
                gameplayMap["Pause"].performed -= OnPause;
            }
            if (uiMap != null)
            {
                uiMap["Navigate"].performed -= OnNavigate;
                uiMap["Submit"].performed -= OnSubmit;
            }
        }

        void OnMove(InputAction.CallbackContext ctx) => MoveEvent?.Invoke(ctx.ReadValue<float>());
        void OnMoveCanceled(InputAction.CallbackContext _) => MoveEvent?.Invoke(0);
        void OnJump(InputAction.CallbackContext _) => JumpEvent?.Invoke();
        void OnJumpCanceled(InputAction.CallbackContext _) => JumpCanceledEvent?.Invoke();
        void OnDash(InputAction.CallbackContext _) => DashEvent?.Invoke();
        void OnAttack(InputAction.CallbackContext _) => AttackEvent?.Invoke();
        void OnInteract(InputAction.CallbackContext _) => InteractEvent?.Invoke();
        void OnPause(InputAction.CallbackContext _) => PauseEvent?.Invoke();
        void OnNavigate(InputAction.CallbackContext ctx) => NavigateEvent?.Invoke(ctx.ReadValue<Vector2>());
        void OnSubmit(InputAction.CallbackContext _) => SubmitEvent?.Invoke();

        public void EnableUIInput() { gameplayMap?.Disable(); uiMap?.Enable(); }
    }
}
