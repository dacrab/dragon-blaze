using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Room room;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            room.ActivateRoom(true);
        }
    }
}
