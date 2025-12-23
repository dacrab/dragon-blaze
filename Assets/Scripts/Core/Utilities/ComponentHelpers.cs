using UnityEngine;
using Gameplay.Characters.Player;
using Gameplay.Health;

namespace Core.Utilities
{
    /// <summary>
    /// Helper methods to reduce GetComponent boilerplate.
    /// </summary>
    public static class ComponentHelpers
    {
        public static PlayerController GetPlayerController(this GameObject go) => go.GetComponent<PlayerController>();
        public static PlayerController GetPlayerController(this Component comp) => comp.GetComponent<PlayerController>();
        
        public static Health GetHealth(this GameObject go) => go.GetComponent<Health>();
        public static Health GetHealth(this Component comp) => comp.GetComponent<Health>();
        
        public static bool TryGetPlayerController(this GameObject go, out PlayerController controller)
        {
            controller = go.GetComponent<PlayerController>();
            return controller != null;
        }
        
        public static bool TryGetHealth(this GameObject go, out Health health)
        {
            health = go.GetComponent<Health>();
            return health != null;
        }
        
        public static bool TryGetHealth(this Component comp, out Health health)
        {
            health = comp.GetComponent<Health>();
            return health != null;
        }
        
        public static bool TryGetPlayerController(this Component comp, out PlayerController controller)
        {
            controller = comp.GetComponent<PlayerController>();
            return controller != null;
        }
        
        public static bool TryGetHealth(this Collider2D collider, out Health health)
        {
            health = collider.GetComponent<Health>();
            return health != null;
        }
        
        public static bool TryGetPlayerController(this Collider2D collider, out PlayerController controller)
        {
            controller = collider.GetComponent<PlayerController>();
            return controller != null;
        }
    }
}

