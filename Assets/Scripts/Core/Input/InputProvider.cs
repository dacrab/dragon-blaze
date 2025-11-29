using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    public class InputProvider : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        private Keyboard keyboard;
        private Mouse mouse;

        private void Awake()
        {
            keyboard = Keyboard.current;
            mouse = Mouse.current;
        }

        private void Update()
        {
            if (inputReader == null) return;

            ProcessMovement();
            ProcessActions();
            ProcessNavigation();
        }

        private void ProcessMovement()
        {
            if (keyboard == null) return;

            float moveX = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveX = -1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveX = 1f;
            inputReader.RaiseMoveEvent(moveX);
        }

        private void ProcessActions()
        {
            if (keyboard != null)
            {
                if (keyboard.spaceKey.wasPressedThisFrame) inputReader.RaiseJumpEvent();
                if (keyboard.spaceKey.wasReleasedThisFrame) inputReader.RaiseJumpCanceledEvent();
                if (keyboard.leftShiftKey.wasPressedThisFrame) inputReader.RaiseDashEvent();
                if (keyboard.eKey.wasPressedThisFrame) inputReader.RaiseInteractEvent();
                if (keyboard.escapeKey.wasPressedThisFrame) inputReader.RaisePauseEvent();
            }

            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                inputReader.RaiseAttackEvent();
        }

        private void ProcessNavigation()
        {
            if (keyboard == null) return;

            var nav = Vector2.zero;
            if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame) nav.y = 1f;
            if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame) nav.y = -1f;
            if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame) nav.x = -1f;
            if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame) nav.x = 1f;

            if (nav != Vector2.zero)
                inputReader.RaiseNavigateEvent(nav);

            if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
                inputReader.RaiseSubmitEvent();
        }
    }
}
