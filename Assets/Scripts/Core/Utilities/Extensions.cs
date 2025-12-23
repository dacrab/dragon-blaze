using UnityEngine;
using System.Collections.Generic;

namespace Core.Utilities
{
    /// <summary>
    /// Extension methods for common Unity operations.
    /// </summary>
    public static class Extensions
    {
        #region Transform Extensions
        /// <summary>
        /// Resets transform position, rotation, and scale to default values.
        /// </summary>
        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Resets local transform values.
        /// </summary>
        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Sets X position while preserving Y and Z.
        /// </summary>
        public static void SetPositionX(this Transform transform, float x)
        {
            var pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }

        /// <summary>
        /// Sets Y position while preserving X and Z.
        /// </summary>
        public static void SetPositionY(this Transform transform, float y)
        {
            var pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        /// <summary>
        /// Sets Z position while preserving X and Y.
        /// </summary>
        public static void SetPositionZ(this Transform transform, float z)
        {
            var pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }
        #endregion

        #region GameObject Extensions
        /// <summary>
        /// Gets component or adds it if it doesn't exist.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Checks if GameObject has a specific component.
        /// </summary>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }

        /// <summary>
        /// Destroys all children of this GameObject.
        /// </summary>
        public static void DestroyChildren(this GameObject gameObject)
        {
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }
        #endregion

        #region List Extensions
        /// <summary>
        /// Returns a random element from the list.
        /// </summary>
        public static T RandomElement<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return default(T);
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Shuffles the list using Fisher-Yates algorithm.
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        #endregion

        #region Vector Extensions
        /// <summary>
        /// Returns Vector2 with X and Y from Vector3.
        /// </summary>
        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        /// <summary>
        /// Returns Vector3 with X and Y from Vector2, Z set to 0.
        /// </summary>
        public static Vector3 ToVector3(this Vector2 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0f);
        }

        /// <summary>
        /// Returns Vector3 with X and Y from Vector2, with custom Z.
        /// </summary>
        public static Vector3 ToVector3(this Vector2 vector2, float z)
        {
            return new Vector3(vector2.x, vector2.y, z);
        }

        /// <summary>
        /// Checks if Vector2 is approximately zero.
        /// </summary>
        public static bool IsApproximatelyZero(this Vector2 vector, float threshold = 0.001f)
        {
            return vector.sqrMagnitude < threshold * threshold;
        }

        /// <summary>
        /// Checks if Vector3 is approximately zero.
        /// </summary>
        public static bool IsApproximatelyZero(this Vector3 vector, float threshold = 0.001f)
        {
            return vector.sqrMagnitude < threshold * threshold;
        }
        #endregion

        #region Color Extensions
        /// <summary>
        /// Sets alpha value of color.
        /// </summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
        #endregion

        #region Component Extensions
        /// <summary>
        /// Sets enabled state of a Behaviour component.
        /// </summary>
        public static void SetEnabled(this Behaviour component, bool enabled)
        {
            if (component != null) component.enabled = enabled;
        }
        #endregion
    }
}

