using UnityEngine;

namespace Gameplay.Items
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Collectable : MonoBehaviour
    {
        public abstract void Collect();
    }
}
