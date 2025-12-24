using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// Attribute to automatically wire up component references at runtime.
    /// Use this instead of manually assigning references in the Inspector.
    /// </summary>
    public class AutoWireAttribute : PropertyAttribute
    {
        public enum WireType
        {
            /// <summary>Get component from this GameObject</summary>
            Self,
            /// <summary>Get component from parent GameObject</summary>
            Parent,
            /// <summary>Get component from children GameObjects</summary>
            Children,
            /// <summary>Find in scene using FindFirstObjectByType</summary>
            Scene,
            /// <summary>Get from ServiceLocator</summary>
            Service,
            /// <summary>Get from PlayerReference utility</summary>
            Player,
            /// <summary>Get component from GameObject with specific tag</summary>
            Tagged
        }

        public WireType Type { get; }
        public string Tag { get; } // For Tagged type
        public bool Required { get; } // Log error if not found

        public AutoWireAttribute(WireType type = WireType.Self, bool required = true)
        {
            Type = type;
            Required = required;
        }

        public AutoWireAttribute(WireType type, string tag, bool required = true)
        {
            Type = type;
            Tag = tag;
            Required = required;
        }
    }
}


