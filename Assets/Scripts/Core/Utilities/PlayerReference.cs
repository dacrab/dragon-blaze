using UnityEngine;
using Core.Constants;
using Gameplay.Characters.Player;

namespace Core.Utilities
{
    public static class PlayerReference
    {
        private static Transform cachedTransform;
        private static PlayerController cachedController;
        private static bool isCached;

        public static Transform Transform
        {
            get
            {
                EnsureCached();
                return cachedTransform;
            }
        }

        public static PlayerController Controller
        {
            get
            {
                EnsureCached();
                return cachedController;
            }
        }

        public static bool IsValid => Transform != null;

        private static void EnsureCached()
        {
            if (isCached && cachedTransform != null) return;

            var player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (player != null)
            {
                cachedTransform = player.transform;
                cachedController = player.GetComponent<PlayerController>();
            }
            isCached = true;
        }

        public static void ClearCache()
        {
            cachedTransform = null;
            cachedController = null;
            isCached = false;
        }

        public static T GetComponent<T>() where T : Component
        {
            return Transform != null ? Transform.GetComponent<T>() : null;
        }
    }
}
