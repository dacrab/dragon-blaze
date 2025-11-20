using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    public class InputProvider : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        private void Update()
        {
            if (inputReader == null)
                return;

            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            if (keyboard != null)
            {
                float moveX = 0f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveX = -1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveX = 1f;
                inputReader.RaiseMoveEvent(moveX);

                if (keyboard.spaceKey.wasPressedThisFrame) inputReader.RaiseJumpEvent();
                if (keyboard.spaceKey.wasReleasedThisFrame) inputReader.RaiseJumpCanceledEvent();

                if (keyboard.leftShiftKey.wasPressedThisFrame) inputReader.RaiseDashEvent();

                if (keyboard.eKey.wasPressedThisFrame) inputReader.RaiseInteractEvent();

                if (keyboard.escapeKey.wasPressedThisFrame) inputReader.RaisePauseEvent();

                Vector2 navInput = Vector2.zero;
                if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame) navInput.y = 1f;
                if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame) navInput.y = -1f;
                if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame) navInput.x = -1f;
                if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame) navInput.x = 1f;
                
                if (navInput != Vector2.zero)
                    inputReader.RaiseNavigateEvent(navInput);
                if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
                    inputReader.RaiseSubmitEvent();
            }
            
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                inputReader.RaiseAttackEvent();
        }
    }
}
