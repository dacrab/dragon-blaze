using UnityEngine;
using Core.Utilities;
using Core.Managers;
using Gameplay.Characters.Player;
using UI.Managers;

namespace Core.Utilities.Examples
{
    /// <summary>
    /// EXAMPLES: How to use AutoWire system
    /// 
    /// Instead of manually dragging references in Inspector, use [AutoWire] attribute!
    /// </summary>
    public class AutoWireExamples : MonoBehaviour
    {
        // Example 1: Get component from this GameObject
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private Rigidbody2D rb;

        // Example 2: Get component from parent
        [AutoWire(AutoWireAttribute.WireType.Parent)]
        [SerializeField] private Transform parentTransform;

        // Example 3: Get component from children
        [AutoWire(AutoWireAttribute.WireType.Children)]
        [SerializeField] private SpriteRenderer childSprite;

        // Example 4: Find in scene (singletons, managers)
        [AutoWire(AutoWireAttribute.WireType.Scene)]
        [SerializeField] private GameManager gameManager;

        // Example 5: Get from ServiceLocator
        [AutoWire(AutoWireAttribute.WireType.Service)]
        [SerializeField] private SoundManager soundManager;

        // Example 6: Get player reference (uses PlayerReference utility)
        [AutoWire(AutoWireAttribute.WireType.Player)]
        [SerializeField] private PlayerController player;

        // Example 7: Get GameObject by tag
        [AutoWire(AutoWireAttribute.WireType.Tagged, "Player")]
        [SerializeField] private GameObject playerObject;

        // Example 8: Optional (won't log warning if not found)
        [AutoWire(AutoWireAttribute.WireType.Scene, required: false)]
        [SerializeField] private UIManager optionalUI;

        private void Awake()
        {
            // Option A: Use AutoWireComponent base class (automatic)
            // Just inherit from AutoWireComponent instead of MonoBehaviour
            
            // Option B: Manual wiring (if not using AutoWireComponent)
            AutoWireHelper.WireAllFields(this);
        }
    }
}

