using UnityEngine;
using Core.Constants;

namespace Environment.Rooms
{

public sealed class Door : MonoBehaviour
{
    [SerializeField] Room room;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tags.Player)) room?.ActivateRoom(true);
    }
}
}