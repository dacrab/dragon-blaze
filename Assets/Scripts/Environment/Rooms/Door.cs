using UnityEngine;
using Core.Constants;

public class Door : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Room room;
    #endregion

    #region Unity Lifecycle Methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandlePlayerCollision(collision);
    }
    #endregion

    #region Private Methods
    private void HandlePlayerCollision(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tags.Player))
        {
            // Check position to ensure player is moving into the new room?
            // Original logic: Simple collision trigger.
            
            // Previous room logic: Previous room should probably deactivate?
            // Original Room/Door logic is: Door connects to a Room. When touched, activate room.
            // Room logic: "DeactivateRoomIfNotFirst" on Awake.
            // It doesn't seem to deactivate OLD rooms when entering new ones.
            // This system seems designed for "activating enemies when entering a room" optimization.
            
            ActivateConnectedRoom();
        }
    }

    private void ActivateConnectedRoom()
    {
        if (room != null)
            room.ActivateRoom(true);
    }
    #endregion
}
