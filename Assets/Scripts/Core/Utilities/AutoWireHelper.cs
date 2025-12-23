using UnityEngine;
using Core.Services;
using Core.Constants;

namespace Core.Utilities
{
    /// <summary>
    /// Helper class for automatic reference wiring at runtime.
    /// Reduces manual Inspector assignment work.
    /// </summary>
    public static class AutoWireHelper
    {
        /// <summary>
        /// Automatically wires a field based on AutoWireAttribute settings.
        /// </summary>
        public static T WireReference<T>(Component component, AutoWireAttribute attribute) where T : Object
        {
            if (component == null) return null;

            T result = null;

            switch (attribute.Type)
            {
                case AutoWireAttribute.WireType.Self:
                    result = component.GetComponent<T>() as T;
                    break;

                case AutoWireAttribute.WireType.Parent:
                    result = component.GetComponentInParent<T>() as T;
                    break;

                case AutoWireAttribute.WireType.Children:
                    result = component.GetComponentInChildren<T>() as T;
                    break;

                case AutoWireAttribute.WireType.Scene:
                    result = Object.FindFirstObjectByType<T>() as T;
                    break;

                case AutoWireAttribute.WireType.Service:
                    result = ServiceLocator.Get<T>();
                    break;

                case AutoWireAttribute.WireType.Player:
                    if (typeof(T) == typeof(Gameplay.Characters.Player.PlayerController))
                    {
                        result = PlayerReference.Controller as T;
                    }
                    else if (typeof(T) == typeof(Transform))
                    {
                        result = PlayerReference.Transform as T;
                    }
                    break;

                case AutoWireAttribute.WireType.Tagged:
                    if (!string.IsNullOrEmpty(attribute.Tag))
                    {
                        var taggedObject = GameObject.FindGameObjectWithTag(attribute.Tag);
                        if (taggedObject != null)
                        {
                            result = taggedObject.GetComponent<T>() as T;
                        }
                    }
                    break;
            }

            if (result == null && attribute.Required)
            {
                Debug.LogWarning($"[AutoWire] Failed to wire {typeof(T).Name} on {component.gameObject.name} using {attribute.Type}");
            }

            return result;
        }

        /// <summary>
        /// Automatically wires all fields marked with AutoWireAttribute on a component.
        /// Call this in Awake() or OnEnable().
        /// </summary>
        public static void WireAllFields(Component component)
        {
            if (component == null) return;

            var fields = component.GetType().GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic
            );

            foreach (var field in fields)
            {
                var attribute = System.Attribute.GetCustomAttribute(field, typeof(AutoWireAttribute)) as AutoWireAttribute;
                if (attribute == null) continue;

                var fieldType = field.FieldType;
                
                // Handle Component types
                if (typeof(Component).IsAssignableFrom(fieldType) && fieldType != typeof(Component))
                {
                    var method = typeof(AutoWireHelper).GetMethod(nameof(WireReference), 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var genericMethod = method.MakeGenericMethod(fieldType);
                    var value = genericMethod.Invoke(null, new object[] { component, attribute });
                    
                    if (value != null)
                    {
                        field.SetValue(component, value);
                    }
                }
                // Handle GameObject
                else if (fieldType == typeof(GameObject))
                {
                    GameObject result = null;
                    switch (attribute.Type)
                    {
                        case AutoWireAttribute.WireType.Self:
                            result = component.gameObject;
                            break;
                        case AutoWireAttribute.WireType.Parent:
                            result = component.transform.parent?.gameObject;
                            break;
                        case AutoWireAttribute.WireType.Tagged:
                            if (!string.IsNullOrEmpty(attribute.Tag))
                            {
                                result = GameObject.FindGameObjectWithTag(attribute.Tag);
                            }
                            break;
                    }
                    if (result != null)
                    {
                        field.SetValue(component, result);
                    }
                }
            }
        }
    }
}

