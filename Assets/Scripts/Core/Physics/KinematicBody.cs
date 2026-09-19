using UnityEngine;

namespace Core.Physics
{
    /// <summary>
    /// Helpers for trap/platform/enemy movers. Static bodies authored in scenes are switched
    /// to Kinematic on prepare so MoveTo can drive them via the physics engine; a missing
    /// Rigidbody2D falls back to direct transform movement.
    /// </summary>
    public static class KinematicBody
    {
        public static Rigidbody2D? Prepare(Component owner)
        {
            var rb = owner.GetComponent<Rigidbody2D>();
            if (rb != null && rb.bodyType == RigidbodyType2D.Static)
                rb.bodyType = RigidbodyType2D.Kinematic;
            return rb;
        }

        public static void MoveTo(Rigidbody2D? rb, Transform transform, Vector3 position)
        {
            if (rb != null) rb.MovePosition(position);
            else transform.position = position;
        }
    }
}
