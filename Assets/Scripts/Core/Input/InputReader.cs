using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Core.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "DragonBlaze/Input/Input Reader")]
    public sealed class InputReader : ScriptableObject
    {
        public const string ResourceKey = nameof(InputReader);

        const string GameplayMap = "Gameplay";
        const string UIMap = "UI";

        [SerializeField] InputActionAsset inputActions;

        public event UnityAction<float> MoveEvent;
        public event UnityAction JumpEvent, JumpCanceledEvent, DashEvent, AttackEvent, InteractEvent, PauseEvent, SubmitEvent;
        public event UnityAction<Vector2> NavigateEvent;

        InputActionMap gameplayMap, uiMap;
        (string name, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)[] gameplayActions;

        static InputReader instance;

        /// <summary>Resolves the shared reader from Resources so no scene wiring is required.</summary>
        public static InputReader Instance => instance != null ? instance : instance = Resources.Load<InputReader>(ResourceKey);

        internal bool HasActionMaps => gameplayMap != null && uiMap != null;

        void OnEnable()
        {
            if (inputActions == null) return;
            gameplayMap = inputActions.FindActionMap(GameplayMap);
            uiMap = inputActions.FindActionMap(UIMap);
            gameplayActions ??= new[]
            {
                ("Move", (Action<InputAction.CallbackContext>)OnMove, (Action<InputAction.CallbackContext>)OnMoveCanceled),
                ("Jump", (Action<InputAction.CallbackContext>)OnJump, (Action<InputAction.CallbackContext>)OnJumpCanceled),
                ("Dash", (Action<InputAction.CallbackContext>)OnDash, null),
                ("Attack", (Action<InputAction.CallbackContext>)OnAttack, null),
                ("Interact", (Action<InputAction.CallbackContext>)OnInteract, null),
                ("Pause", (Action<InputAction.CallbackContext>)OnPause, null),
            };

            if (gameplayMap != null)
            {
                gameplayMap.Enable();
                foreach (var (name, performed, canceled) in gameplayActions)
                {
                    gameplayMap[name].performed += performed;
                    if (canceled != null) gameplayMap[name].canceled += canceled;
                }
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
                foreach (var (name, performed, canceled) in gameplayActions)
                {
                    gameplayMap[name].performed -= performed;
                    if (canceled != null) gameplayMap[name].canceled -= canceled;
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
        public void EnableGameplayInput() { gameplayMap?.Enable(); uiMap?.Enable(); }
    }
}
