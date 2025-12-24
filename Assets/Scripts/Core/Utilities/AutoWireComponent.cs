using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// Base component that automatically wires references marked with [AutoWire] attribute.
    /// Inherit from this instead of MonoBehaviour to get auto-wiring functionality.
    /// </summary>
    public abstract class AutoWireComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            AutoWireHelper.WireAllFields(this);
        }
    }
}


