using UnityEngine;
using Core.Constants;

namespace Environment.Rooms
{
    public class Door : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Room room;
        #endregion

        #region Unity Lifecycle Methods
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) room?.ActivateRoom(true);
        }
        #endregion
    }
}
