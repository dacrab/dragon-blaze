using UnityEngine;
using Core.Constants;

namespace Environment.Rooms
{
    public sealed class Room : MonoBehaviour
    {
        [SerializeField] GameObject[] enemies;
        [SerializeField] bool startActive;
        Vector3[] initialPositions;

        void Awake()
        {
            initialPositions = new Vector3[enemies.Length];
            for (int i = 0; i < enemies.Length; i++)
                initialPositions[i] = enemies[i] != null ? enemies[i].transform.position : Vector3.zero;
            ActivateRoom(startActive);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) ActivateRoom(true);
        }

        void ActivateRoom(bool status)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null) continue;
                enemies[i].SetActive(status);
                if (status) enemies[i].transform.position = initialPositions[i];
            }
        }
    }
}
