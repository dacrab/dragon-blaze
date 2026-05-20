using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core.Loading
{
    /// <summary>
    /// Async asset loader using Addressables. Handles loading/unloading with reference counting.
    /// Usage: await AssetLoader.LoadAssetAsync&lt;GameObject&gt;("EnemyPrefab");
    /// </summary>
    public static class AssetLoader
    {
        static readonly Dictionary<string, AsyncOperationHandle> handles = new();

        public static async Awaitable<T> LoadAssetAsync<T>(string key) where T : Object
        {
            if (handles.TryGetValue(key, out var existing))
                return (T)existing.Result;

            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                handles[key] = handle;
                return handle.Result;
            }

            Debug.LogError($"[AssetLoader] Failed to load: {key}");
            return null;
        }

        public static async Awaitable<T> InstantiateAsync<T>(string key, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            var prefab = await LoadAssetAsync<GameObject>(key);
            if (prefab == null) return null;
            var instance = Object.Instantiate(prefab, position, rotation, parent);
            return instance.GetComponent<T>();
        }

        public static async Awaitable<SceneInstance> LoadSceneAsync(string key, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var handle = Addressables.LoadSceneAsync(key, mode);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                return handle.Result;

            Debug.LogError($"[AssetLoader] Failed to load scene: {key}");
            return default;
        }

        public static void Release(string key)
        {
            if (!handles.Remove(key, out var handle)) return;
            Addressables.Release(handle);
        }

        public static void ReleaseAll()
        {
            foreach (var handle in handles.Values)
                Addressables.Release(handle);
            handles.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => handles.Clear();
    }
}
