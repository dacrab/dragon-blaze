using UnityEngine;
using UnityEngine.Events;
using Core.Constants;

namespace Core.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "DragonBlaze/Input/Input Reader")]
    public class InputReader : ScriptableObject
    {
        // Events
        public event UnityAction<float> MoveEvent;
        public event UnityAction JumpEvent;
        public event UnityAction JumpCanceledEvent;
        public event UnityAction DashEvent;
        public event UnityAction PauseEvent;
        public event UnityAction InteractEvent;

        public void EnableInput()
        {
            // If using new Input System, we would enable action maps here
        }

        public void DisableInput()
        {
            // If using new Input System, we would disable action maps here
        }

        // This method will be called by a MonoBehaviour (e.g., InputManager) that sits in the scene and reads the legacy Input
        // Or if we switch to New Input System, this SO can implement the Interface
        
        // For Legacy Input Interop
        public void ProcessInput()
        {
            float horizontal = UnityEngine.Input.GetAxis(GameConstants.Input.Horizontal);
            MoveEvent?.Invoke(horizontal);

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                JumpEvent?.Invoke();
            }
            if (UnityEngine.Input.GetKeyUp(KeyCode.Space))
            {
                JumpCanceledEvent?.Invoke();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
            {
                DashEvent?.Invoke();
            }
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                PauseEvent?.Invoke();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                InteractEvent?.Invoke();
            }
        }
    }
}
