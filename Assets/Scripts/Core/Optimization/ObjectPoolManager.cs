using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Core.Managers;
using Core.Services;
using Core.Interfaces;

namespace Core.Optimization
{
    /// <summary>
    /// Modern object pool manager using Unity's built-in ObjectPool system.
    /// More efficient and expandable than custom Queue-based implementation.
    /// </summary>
    public class ObjectPoolManager : SingletonManager<ObjectPoolManager>
    {
        [System.Serializable]
        public class PoolConfig
        {
            public string tag;
            public GameObject prefab;
            public int initialSize = 10;
            public int maxSize = 50;
            public bool collectionCheck = true; // Prevents returning already pooled objects
        }

        [SerializeField] private List<PoolConfig> poolConfigs;
        
        private Dictionary<string, IObjectPool<GameObject>> pools;
        private Dictionary<string, PoolConfig> configLookup;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            InitializePools();
            ServiceLocator.Register<ObjectPoolManager>(this);
        }

        protected override void OnShutdown()
        {
            ClearAllPools();
            ServiceLocator.Unregister<ObjectPoolManager>();
            base.OnShutdown();
        }

        private void InitializePools()
        {
            pools = new Dictionary<string, IObjectPool<GameObject>>(poolConfigs.Count);
            configLookup = new Dictionary<string, PoolConfig>(poolConfigs.Count);

            foreach (var config in poolConfigs)
            {
                configLookup[config.tag] = config;
                
                var pool = new ObjectPool<GameObject>(
                    createFunc: () => CreatePooledObject(config.prefab),
                    actionOnGet: OnGetFromPool,
                    actionOnRelease: OnReturnToPool,
                    actionOnDestroy: OnDestroyPooledObject,
                    collectionCheck: config.collectionCheck,
                    defaultCapacity: config.initialSize,
                    maxSize: config.maxSize
                );
                
                pools[config.tag] = pool;
                
                // Pre-warm the pool
                var preWarmObjects = new List<GameObject>(config.initialSize);
                for (int i = 0; i < config.initialSize; i++)
                {
                    preWarmObjects.Add(pool.Get());
                }
                foreach (var obj in preWarmObjects)
                {
                    pool.Release(obj);
                }
            }
        }

        private GameObject CreatePooledObject(GameObject prefab)
        {
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            return obj;
        }

        private void OnGetFromPool(GameObject obj)
        {
            obj.SetActive(true);
            if (obj.TryGetComponent<IPoolable>(out var poolable))
                poolable.OnSpawn();
        }

        private void OnReturnToPool(GameObject obj)
        {
            if (obj.TryGetComponent<IPoolable>(out var poolable))
                poolable.OnDespawn();
            obj.SetActive(false);
        }

        private void OnDestroyPooledObject(GameObject obj)
        {
            Destroy(obj);
        }

        /// <summary>
        /// Gets an object from the pool. Returns null if pool doesn't exist.
        /// </summary>
        public GameObject Get(string tag, Vector3 position, Quaternion rotation)
        {
            if (!pools.TryGetValue(tag, out var pool))
            {
                Debug.LogWarning($"[ObjectPoolManager] Pool '{tag}' not found.");
                return null;
            }

            var obj = pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        public void Release(string tag, GameObject obj)
        {
            if (obj == null) return;
            
            if (pools.TryGetValue(tag, out var pool))
            {
                pool.Release(obj);
            }
            else
            {
                Debug.LogWarning($"[ObjectPoolManager] Pool '{tag}' not found. Destroying object instead.");
                Destroy(obj);
            }
        }

        /// <summary>
        /// Gets the pool for a specific tag. Useful for advanced pooling operations.
        /// </summary>
        public IObjectPool<GameObject> GetPool(string tag)
        {
            pools.TryGetValue(tag, out var pool);
            return pool;
        }

        /// <summary>
        /// Creates a new pool at runtime. Useful for dynamic pooling needs.
        /// </summary>
        public void CreatePool(string tag, GameObject prefab, int initialSize = 10, int maxSize = 50)
        {
            if (pools.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPoolManager] Pool '{tag}' already exists.");
                return;
            }

            var config = new PoolConfig
            {
                tag = tag,
                prefab = prefab,
                initialSize = initialSize,
                maxSize = maxSize
            };

            configLookup[tag] = config;
            
            var pool = new ObjectPool<GameObject>(
                createFunc: () => CreatePooledObject(prefab),
                actionOnGet: OnGetFromPool,
                actionOnRelease: OnReturnToPool,
                actionOnDestroy: OnDestroyPooledObject,
                collectionCheck: true,
                defaultCapacity: initialSize,
                maxSize: maxSize
            );
            
            pools[tag] = pool;
        }

        /// <summary>
        /// Clears all pools. Use with caution.
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }
            pools.Clear();
            configLookup.Clear();
        }

        /// <summary>
        /// Gets the count of active objects in a pool.
        /// </summary>
        public int GetActiveCount(string tag)
        {
            if (pools.TryGetValue(tag, out var pool) && pool is ObjectPool<GameObject> objectPool)
            {
                return objectPool.CountActive;
            }
            return -1;
        }

        /// <summary>
        /// Gets the count of inactive objects in a pool.
        /// </summary>
        public int GetInactiveCount(string tag)
        {
            if (pools.TryGetValue(tag, out var pool) && pool is ObjectPool<GameObject> objectPool)
            {
                return objectPool.CountInactive;
            }
            return -1;
        }
    }
}
