using UnityEngine;
using Core.Optimization;

namespace Core.Utilities
{
    /// <summary>
    /// Extension methods for easier pool usage.
    /// Makes the codebase more expandable and easier to use.
    /// </summary>
    public static class PoolExtensions
    {
        /// <summary>
        /// Gets an object from the pool at the specified position.
        /// </summary>
        public static GameObject Spawn(this ObjectPoolManager poolManager, string tag, Vector3 position)
        {
            return poolManager.Get(tag, position, Quaternion.identity);
        }

        /// <summary>
        /// Gets an object from the pool at the specified position and rotation.
        /// </summary>
        public static GameObject Spawn(this ObjectPoolManager poolManager, string tag, Vector3 position, Quaternion rotation)
        {
            return poolManager.Get(tag, position, rotation);
        }

        /// <summary>
        /// Gets an object from the pool at this transform's position and rotation.
        /// </summary>
        public static GameObject Spawn(this ObjectPoolManager poolManager, string tag, Transform parent)
        {
            var obj = poolManager.Get(tag, parent.position, parent.rotation);
            if (obj != null) obj.transform.SetParent(parent);
            return obj;
        }

        /// <summary>
        /// Returns this GameObject to the specified pool.
        /// </summary>
        public static void ReturnToPool(this GameObject obj, string tag)
        {
            ObjectPoolManager.Instance?.Release(tag, obj);
        }

        /// <summary>
        /// Returns this GameObject to the pool after a delay.
        /// </summary>
        public static void ReturnToPoolAfter(this GameObject obj, string tag, float delay)
        {
            if (obj.TryGetComponent<MonoBehaviour>(out var mb))
            {
                mb.StartCoroutine(ReturnAfterDelay(obj, tag, delay));
            }
        }

        private static System.Collections.IEnumerator ReturnAfterDelay(GameObject obj, string tag, float delay)
        {
            yield return new WaitForSeconds(delay);
            obj.ReturnToPool(tag);
        }
    }
}

