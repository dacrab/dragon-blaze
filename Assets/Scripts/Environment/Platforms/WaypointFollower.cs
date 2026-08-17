using UnityEngine;

namespace Environment.Platforms
{
    public sealed class WaypointFollower : MonoBehaviour
    {
        [SerializeField] Transform[] waypoints;
        [SerializeField] float speed = 2f;
        [SerializeField] bool loop = true, pingPong;

        int currentIndex, direction = 1;

        void Update()
        {
            if (waypoints is not { Length: > 0 }) return;
            var target = waypoints[currentIndex].position;
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * speed);
            if (((Vector2)transform.position - target).sqrMagnitude < 0.01f) AdvanceWaypoint();
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
