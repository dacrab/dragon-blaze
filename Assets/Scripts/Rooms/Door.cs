using UnityEngine;

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
        if (collision.CompareTag("Player"))
        {
            ActivateConnectedRoom();
        }
    }

    private void ActivateConnectedRoom()
    {
        room.ActivateRoom(true);
    }
    #endregion
}
