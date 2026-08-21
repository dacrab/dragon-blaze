using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.Pooling
{
    public sealed class GameObjectPool : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        [SerializeField] string poolKey;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxSize = 50;
        [SerializeField] bool prewarm;

        ObjectPool<GameObject> pool;

        void Awake()
        {
            pool = new ObjectPool<GameObject>(
                createFunc: () => { var obj = Instantiate(prefab, transform); obj.SetActive(false); return obj; },
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: Destroy,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );

            if (prewarm)
                for (int i = 0; i < defaultCapacity; i++)
                    pool.Release(pool.Get());

            if (!string.IsNullOrEmpty(poolKey)) PoolRegistry.Register(poolKey, this);
        }

        void OnDestroy() { if (!string.IsNullOrEmpty(poolKey)) PoolRegistry.Unregister(poolKey); }

        public GameObject Get() => pool.Get();
        public void Release(GameObject obj) => pool.Release(obj);
    }

    public static class PoolRegistry
    {
        static readonly Dictionary<string, GameObjectPool> pools = new();

        public static void Register(string key, GameObjectPool pool) => pools[key] = pool;
        public static void Unregister(string key) => pools.Remove(key);

        public static void Release(string key, GameObject obj)
        {
            if (pools.TryGetValue(key, out var pool)) pool.Release(obj);
            else obj.SetActive(false);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => pools.Clear();
    }
}
