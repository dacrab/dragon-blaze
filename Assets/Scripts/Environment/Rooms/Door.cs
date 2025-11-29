using UnityEngine;
using Core.Constants;

namespace Environment.Rooms
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Room room;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player))
                room?.SetActive(true);
        }
    }
}
