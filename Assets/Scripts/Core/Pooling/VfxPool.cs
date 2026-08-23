using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.Pooling
{
    public static class VfxPool
    {
        static readonly Dictionary<int, ObjectPool<GameObject>> pools = new();
        static Transform root;

        static Transform Root => root != null ? root : root = CreateRoot();

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null) return null;
            var obj = GetPool(prefab).Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }

        public static void Release(GameObject prefab, GameObject obj) => GetPool(prefab).Release(obj);

        static ObjectPool<GameObject> GetPool(GameObject prefab)
        {
            int id = prefab.GetInstanceID();
            if (pools.TryGetValue(id, out var pool)) return pool;

            pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Object.Instantiate(prefab, Root);
                    var recycler = obj.GetComponent<VfxRecycler>();
                    if (recycler == null) recycler = obj.AddComponent<VfxRecycler>();
                    recycler.Init(prefab);
                    return obj;
                },
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: Object.Destroy,
                defaultCapacity: 4,
                maxSize: 32
            );
            pools.Add(id, pool);
            return pool;
        }

        static Transform CreateRoot()
        {
            var go = new GameObject("VfxPool");
            Object.DontDestroyOnLoad(go);
            return go.transform;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => pools.Clear();
    }

    public sealed class VfxRecycler : MonoBehaviour
    {
        const float PollInterval = 0.25f;

        GameObject prefab;
        ParticleSystem particles;
        float duration;
        CancellationTokenSource recycleCts;

        public void Init(GameObject prefab)
        {
            this.prefab = prefab;
            particles = GetComponent<ParticleSystem>();
            duration = particles != null
                ? Mathf.Max(particles.main.duration, particles.main.startLifetime.constantMax)
                : 1f;
        }

        void OnEnable()
        {
            if (particles != null) particles.Play();
            recycleCts?.Cancel();
            recycleCts = new CancellationTokenSource();
            _ = ReleaseWhenDeadAsync(recycleCts.Token);
        }

        void OnDisable() => recycleCts?.Cancel();

        async Awaitable ReleaseWhenDeadAsync(CancellationToken ct)
        {
            try
            {
                float minEndTime = Time.time + duration;
                while (Time.time < minEndTime)
                    await Awaitable.WaitForSecondsAsync(PollInterval, ct);
                while (particles != null && particles.IsAlive(true))
                    await Awaitable.WaitForSecondsAsync(PollInterval, ct);
                VfxPool.Release(prefab, gameObject);
            }
            catch (OperationCanceledException) { }
        }
    }
}
