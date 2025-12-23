using UnityEngine;
using Core.Interfaces;

namespace Gameplay.Items
{
    /// <summary>
    /// Base class for all collectible items in the game.
    /// </summary>
    public abstract class Collectable : MonoBehaviour
    {
        [Header("Collection Settings")]
        [SerializeField] protected bool destroyOnCollect = true;
        [SerializeField] protected float collectionDelay = 0f;

        /// <summary>
        /// Called when the item is collected. Override to implement collection logic.
        /// </summary>
        public abstract void Collect();

        /// <summary>
        /// Handles the collection process with optional delay.
        /// </summary>
        protected virtual void HandleCollection()
        {
            Collect();
            
            if (collectionDelay > 0f)
            {
                Invoke(nameof(DestroyCollectable), collectionDelay);
            }
            else if (destroyOnCollect)
            {
                DestroyCollectable();
            }
        }

        private void DestroyCollectable()
        {
            if (destroyOnCollect)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
