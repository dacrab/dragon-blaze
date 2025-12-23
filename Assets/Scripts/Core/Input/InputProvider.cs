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
            var kb = Keyboard.current;
            var mouse = Mouse.current;

            if (kb != null)
            {
                float moveX = 0;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) moveX = -1;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveX = 1;
                inputReader.RaiseMoveEvent(moveX);

                if (kb.spaceKey.wasPressedThisFrame) inputReader.RaiseJumpEvent();
                if (kb.spaceKey.wasReleasedThisFrame) inputReader.RaiseJumpCanceledEvent();
                if (kb.leftShiftKey.wasPressedThisFrame) inputReader.RaiseDashEvent();
                if (kb.eKey.wasPressedThisFrame) inputReader.RaiseInteractEvent();
                if (kb.escapeKey.wasPressedThisFrame) inputReader.RaisePauseEvent();
            }
            if (mouse?.leftButton.wasPressedThisFrame == true) inputReader.RaiseAttackEvent();
        }
    }
}
