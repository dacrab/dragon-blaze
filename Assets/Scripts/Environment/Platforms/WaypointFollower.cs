using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private float speed = 2f;
    #endregion

    #region Private Fields
    private int currentWaypointIndex = 0;
    #endregion

    #region Unity Lifecycle Methods
    private void Update()
    {
        UpdateWaypointIndex();
        MoveTowardsWaypoint();
    }
    #endregion

    #region Private Methods
    private void UpdateWaypointIndex()
    {
        if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }
    }

    private void MoveTowardsWaypoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, 
            waypoints[currentWaypointIndex].transform.position, 
            Time.deltaTime * speed);
    }
    #endregion
}
