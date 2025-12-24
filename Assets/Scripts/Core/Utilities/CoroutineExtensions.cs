using System.Collections;
using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// Extension methods for safe coroutine management.
    /// Prevents errors from stopping null coroutines.
    /// </summary>
    public static class CoroutineExtensions
    {
        /// <summary>
        /// Safely stops a coroutine if it's not null.
        /// </summary>
        public static void SafeStopCoroutine(this MonoBehaviour behaviour, ref Coroutine coroutine)
        {
            if (coroutine != null && behaviour != null)
            {
                behaviour.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        /// <summary>
        /// Stops multiple coroutines safely.
        /// </summary>
        public static void SafeStopCoroutines(this MonoBehaviour behaviour, params Coroutine[] coroutines)
        {
            if (behaviour == null) return;
            
            foreach (var coroutine in coroutines)
            {
                if (coroutine != null)
                {
                    behaviour.StopCoroutine(coroutine);
                }
            }
        }
    }
}

