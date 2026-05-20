using UnityEngine;

namespace Core.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 WithX(this Vector2 v, float x) => new(x, v.y);
        public static Vector2 WithY(this Vector2 v, float y) => new(v.x, y);
        public static Vector3 WithX(this Vector3 v, float x) => new(x, v.y, v.z);
        public static Vector3 WithY(this Vector3 v, float y) => new(v.x, y, v.z);
        public static Vector3 WithZ(this Vector3 v, float z) => new(v.x, v.y, z);
        public static Vector2 Flat(this Vector3 v) => new(v.x, v.y);
    }

    public static class TransformExtensions
    {
        public static float DistanceTo(this Transform t, Transform other) =>
            Vector2.Distance(t.position, other.position);

        public static float DirectionTo(this Transform t, Transform other) =>
            Mathf.Sign(other.position.x - t.position.x);
    }

    public static class ComponentExtensions
    {
        public static bool TryGetComponentInParent<T>(this Component c, out T result) where T : class
        {
            result = c.GetComponentInParent<T>();
            return result != null;
        }
    }
}
