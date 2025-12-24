using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// Helper methods for common validation patterns.
    /// Reduces boilerplate null checks and validation code.
    /// </summary>
    public static class ValidationHelpers
    {
        /// <summary>
        /// Validates that a required component exists, logs error if missing.
        /// </summary>
        public static bool ValidateComponent<T>(this MonoBehaviour behaviour, T component, string componentName = null) where T : class
        {
            if (component != null) return true;
            
            string name = componentName ?? typeof(T).Name;
            Debug.LogError($"[{behaviour.GetType().Name}] Required component '{name}' is missing on {behaviour.gameObject.name}!");
            return false;
        }

        /// <summary>
        /// Validates that a required GameObject reference exists.
        /// </summary>
        public static bool ValidateReference<T>(this MonoBehaviour behaviour, T reference, string referenceName) where T : class
        {
            if (reference != null) return true;
            
            Debug.LogError($"[{behaviour.GetType().Name}] Required reference '{referenceName}' is null on {behaviour.gameObject.name}!");
            return false;
        }

        /// <summary>
        /// Validates that a serialized field is assigned in Inspector.
        /// </summary>
        public static bool ValidateSerializedField<T>(this MonoBehaviour behaviour, T field, string fieldName) where T : Object
        {
            if (field != null) return true;
            
            Debug.LogWarning($"[{behaviour.GetType().Name}] Serialized field '{fieldName}' is not assigned on {behaviour.gameObject.name}. Check Inspector!");
            return false;
        }

        /// <summary>
        /// Validates that a value is within a valid range.
        /// </summary>
        public static bool ValidateRange(this MonoBehaviour behaviour, float value, float min, float max, string valueName)
        {
            if (value >= min && value <= max) return true;
            
            Debug.LogWarning($"[{behaviour.GetType().Name}] {valueName} ({value}) is outside valid range [{min}, {max}] on {behaviour.gameObject.name}!");
            return false;
        }
    }
}

