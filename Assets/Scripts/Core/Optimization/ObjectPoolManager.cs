using System.Collections.Generic;
using UnityEngine;

namespace Core.Optimization
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [SerializeField] private List<Pool> pools;
        
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>(pools.Count);

            foreach (var pool in pools)
            {
                var queue = new Queue<GameObject>(pool.size);
                for (int i = 0; i < pool.size; i++)
                {
                    var obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    queue.Enqueue(obj);
                }
                poolDictionary[pool.tag] = queue;
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.TryGetValue(tag, out var queue))
            {
                Debug.LogWarning($"Pool '{tag}' not found.");
                return null;
            }

            var obj = queue.Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            queue.Enqueue(obj);

            return obj;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            if (poolDictionary.TryGetValue(tag, out var queue))
            {
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
        }
    }
}
