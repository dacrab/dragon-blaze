using UnityEngine;
using Core.Constants;

namespace Environment.Rooms
{
    /// <summary>Room boundary: touching it starts the parent room's fight.</summary>
    public sealed class Door : MonoBehaviour
    {
        [SerializeField] Room? room;

        void Awake()
        {
            if (room == null) room = GetComponentInParent<Room>();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) room?.ActivateRoom(true);
        }
    }
}
