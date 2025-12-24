using UnityEngine;

namespace Core.Constants
{
    /// <summary>
    /// Centralized layer constants to avoid hardcoded layer indices.
    /// </summary>
    public static class LayerConstants
    {
        // Layer names - use these with LayerMask.NameToLayer()
        public const string GroundLayer = "Ground";
        public const string PlayerLayer = "Player";
        public const string EnemyLayer = "Enemy";
        public const string ProjectileLayer = "Projectile";
        public const string DefaultLayer = "Default";
        public const string IgnoreRaycastLayer = "Ignore Raycast";

        // Cached layer indices (initialized on first access)
        private static int? _groundLayerIndex;
        private static int? _playerLayerIndex;
        private static int? _enemyLayerIndex;
        private static int? _projectileLayerIndex;

        public static int Ground => _groundLayerIndex ??= GetLayerIndex(GroundLayer);
        public static int Player => _playerLayerIndex ??= GetLayerIndex(PlayerLayer);
        public static int Enemy => _enemyLayerIndex ??= GetLayerIndex(EnemyLayer);
        public static int Projectile => _projectileLayerIndex ??= GetLayerIndex(ProjectileLayer);
        
        /// <summary>
        /// Gets a layer index and validates it exists.
        /// </summary>
        private static int GetLayerIndex(string layerName)
        {
            int index = LayerMask.NameToLayer(layerName);
            if (index == -1)
            {
                Debug.LogWarning($"[LayerConstants] Layer '{layerName}' not found. Make sure it exists in Project Settings > Tags and Layers.");
            }
            return index;
        }

        /// <summary>
        /// Gets a LayerMask for the specified layer name.
        /// </summary>
        public static int GetMask(string layerName) => LayerMask.GetMask(layerName);

        /// <summary>
        /// Gets a combined LayerMask for multiple layers.
        /// </summary>
        public static int GetMask(params string[] layerNames) => LayerMask.GetMask(layerNames);

        /// <summary>
        /// Clears cached layer indices. Call if layers are modified at runtime.
        /// </summary>
        public static void ClearCache()
        {
            _groundLayerIndex = null;
            _playerLayerIndex = null;
            _enemyLayerIndex = null;
            _projectileLayerIndex = null;
        }
    }
}
