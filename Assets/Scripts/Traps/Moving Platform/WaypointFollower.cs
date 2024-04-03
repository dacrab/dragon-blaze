using UnityEngine;

public class WaypointFollower : MonoBehaviour
{        
    [SerializeField] private GameObject[] waypoints; // Array of GameObjects representing the waypoints
    private int currentWaypointIndex = 0; // Index of the current waypoint being followed

    [SerializeField] private float speed = 2f; // Speed at which the object moves between waypoints

    private void Update()
    {
        // Check if the object is close enough to the current waypoint
        if(Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < .1f)
        {
            // Move to the next waypoint
            currentWaypointIndex++;

            // If reached the end of the waypoints array, loop back to the beginning
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }

        // Move the object towards the current waypoint using linear interpolation
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, Time.deltaTime * speed);
    }
}
