using UnityEngine;
using Core.Physics;

namespace Environment.Platforms
{
    public sealed class WaypointFollower : MonoBehaviour
    {
        [SerializeField] Transform[] waypoints;
        [SerializeField] float speed = 2f;
        [SerializeField] bool loop = true, pingPong;

        int currentIndex, direction = 1;
        Rigidbody2D body;

        void Awake() => body = KinematicBody.Prepare(this);

        void FixedUpdate()
        {
            if (waypoints is not { Length: > 0 }) return;
            var target = waypoints[currentIndex].position;
            var current = body != null ? body.position : (Vector2)transform.position;
            var next = Vector2.MoveTowards(current, target, speed * Time.fixedDeltaTime);
            KinematicBody.MoveTo(body, transform, new(next.x, next.y, transform.position.z));
            if ((next - (Vector2)target).sqrMagnitude < 0.01f) AdvanceWaypoint();
        }

        void AdvanceWaypoint()
        {
            if (pingPong)
            {
                currentIndex = Mathf.Clamp(currentIndex + direction, 0, waypoints.Length - 1);
                if (currentIndex >= waypoints.Length - 1 || currentIndex <= 0) direction *= -1;
            }
            else if (loop) currentIndex = (currentIndex + 1) % waypoints.Length;
            else currentIndex = Mathf.Min(currentIndex + 1, waypoints.Length - 1);
        }
    }
}
