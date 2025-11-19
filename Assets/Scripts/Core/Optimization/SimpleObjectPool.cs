using System.Collections.Generic;
using UnityEngine;

namespace Core.Optimization
{
    public class SimpleObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public static SimpleObjectPool instance;
        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);

            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform); // Keep hierarchy clean
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            // If active, we might need to grow pool or skip. 
            // Simple implementation: Re-enqueue immediately at back.
            // Better: Instantiate new if empty.
            if (objectToSpawn.activeInHierarchy)
            {
                 // Optional: Grow pool
                 // For now, just reuse oldest (might look weird if it vanishes) or better: create temp
                 objectToSpawn.SetActive(false); 
            }

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // Re-enqueue
            poolDictionary[tag].Enqueue(objectToSpawn);

            // Trigger any OnSpawn interface if exists
            IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
            if (pooledObj != null)
            {
                pooledObj.OnObjectSpawn();
            }

            return objectToSpawn;
        }
    }

    public interface IPooledObject
    {
        void OnObjectSpawn();
    }
}
