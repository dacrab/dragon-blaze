using UnityEngine;

namespace Core.Physics
{
    public static class KinematicBody
    {
        public static Rigidbody2D Prepare(Component owner)
        {
            var rb = owner.GetComponent<Rigidbody2D>();
            if (rb != null && rb.bodyType == RigidbodyType2D.Static)
                rb.bodyType = RigidbodyType2D.Kinematic;
            return rb;
        }

        public static void MoveTo(Rigidbody2D rb, Transform transform, Vector3 position)
        {
            if (rb != null) rb.MovePosition(position);
            else transform.position = position;
        }
    }
}
