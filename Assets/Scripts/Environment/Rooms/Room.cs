using UnityEngine;
using Core.Constants;
using System.Linq;

namespace Environment.Rooms
{
    public sealed class Room : MonoBehaviour
    {
        [SerializeField] GameObject[] enemies;
        Vector3[] initialPositions;

        void Awake()
        {
            enemies = enemies.Where(e => e != null).ToArray();
            initialPositions = enemies.Select(e => e.transform.position).ToArray();
            
            if (transform.GetSiblingIndex() != 0) ActivateRoom(false);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) ActivateRoom(true);
        }

        public void ActivateRoom(bool status)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(status);
                if (status) enemies[i].transform.position = initialPositions[i];
            }
        }
    }
}
}