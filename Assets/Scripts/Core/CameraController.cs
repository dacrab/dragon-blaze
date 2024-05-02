using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Room camera parameters
    [Header("Room Camera")]
    [SerializeField] private float roomCameraSpeed;
    private float currentRoomPosX;
    private Vector3 roomCameraVelocity = Vector3.zero;

    // Follow player parameters
    [Header("Follow Player")]
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    private float lookAhead;

    private void Update()
    {
        // Room camera movement
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentRoomPosX, transform.position.y, transform.position.z), ref roomCameraVelocity, roomCameraSpeed);
    }

    // Move camera to a new room
    public void MoveToNewRoom(Transform newRoom)
    {
        currentRoomPosX = newRoom.position.x;
    }
}
