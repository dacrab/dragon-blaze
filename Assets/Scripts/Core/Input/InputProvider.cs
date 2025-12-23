using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    public class InputProvider : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        private void Update()
        {
            if (inputReader == null) return;

            // Gameplay Polling
            if (Keyboard.current != null)
            {
                // Move
                float moveX = 0;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1;
                inputReader.RaiseMoveEvent(moveX);

                // Jump
                if (Keyboard.current.spaceKey.wasPressedThisFrame) inputReader.RaiseJumpEvent();
                if (Keyboard.current.spaceKey.wasReleasedThisFrame) inputReader.RaiseJumpCanceledEvent();

                // Dash
                if (Keyboard.current.leftShiftKey.wasPressedThisFrame) inputReader.RaiseDashEvent();

                // Interact
                if (Keyboard.current.eKey.wasPressedThisFrame) inputReader.RaiseInteractEvent();
                
                // Pause
                if (Keyboard.current.escapeKey.wasPressedThisFrame) inputReader.RaisePauseEvent();
            }
            
            if (Mouse.current != null)
            {
                // Attack
                if (Mouse.current.leftButton.wasPressedThisFrame) inputReader.RaiseAttackEvent();
            }
        }
    }
}
