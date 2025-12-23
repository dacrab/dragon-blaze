using UnityEngine;

namespace Environment.Platforms
{
    public class WaypointFollower : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float speed = 2f;
        [SerializeField] private bool loop = true;
        [SerializeField] private bool pingPong;

        private int currentIndex;
        private int direction = 1;

        private void Update()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            var target = waypoints[currentIndex].position;
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * speed);

            if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                AdvanceWaypoint();
            }
        }

        private void AdvanceWaypoint()
        {
            if (pingPong)
            {
                currentIndex += direction;
                if (currentIndex >= waypoints.Length - 1 || currentIndex <= 0) direction *= -1;
            }
            else if (loop) currentIndex = (currentIndex + 1) % waypoints.Length;
            else currentIndex = Mathf.Min(currentIndex + 1, waypoints.Length - 1);
        }
    }
}
