using UnityEngine;

namespace Gameplay.Items
{
    public abstract class Collectable : MonoBehaviour
    {
        [SerializeField] protected bool destroyOnCollect = true;

        public abstract void Collect();
    }
}
