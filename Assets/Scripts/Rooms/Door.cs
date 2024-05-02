using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private Camera mainCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.transform.position.x < transform.position.x)
            {
                MoveToNextRoom();
            }
            else
            {
                MoveToPreviousRoom();
            }
        }
    }

    private void MoveToNextRoom()
    {
        // Move the camera to the next room
        mainCamera.transform.position = nextRoom.position;

        // Ensure the camera is properly aligned with the room
        mainCamera.orthographicSize = CalculateCameraSize(nextRoom);

        // Activate the next room and deactivate the previous one
        nextRoom.GetComponent<Room>().ActivateRoom(true);
        previousRoom.GetComponent<Room>().ActivateRoom(false);
    }

    private void MoveToPreviousRoom()
    {
        // Move the camera to the previous room
        mainCamera.transform.position = previousRoom.position;

        // Ensure the camera is properly aligned with the room
        mainCamera.orthographicSize = CalculateCameraSize(previousRoom);

        // Activate the previous room and deactivate the next one
        previousRoom.GetComponent<Room>().ActivateRoom(true);
        nextRoom.GetComponent<Room>().ActivateRoom(false);
    }

    private float CalculateCameraSize(Transform room)
    {
        // Calculate the size of the camera based on the room's bounds
        Bounds roomBounds = room.GetComponentInChildren<Renderer>().bounds;
        float cameraSize = Mathf.Max(roomBounds.size.x / 2f, roomBounds.size.y / 2f);
        cameraSize += 2f; // Add some padding to ensure the entire room is visible
        return cameraSize;
    }
}
