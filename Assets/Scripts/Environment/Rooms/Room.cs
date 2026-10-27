using UnityEngine;
using Core.Constants;
using Gameplay.Combat;

namespace Environment.Rooms
{
    public sealed class Room : MonoBehaviour
    {
        [SerializeField] GameObject[] enemies;
        [SerializeField] bool startActive;
        Vector3[] initialPositions;

        void Awake()
        {
            if (enemies == null) enemies = System.Array.Empty<GameObject>();
            initialPositions = new Vector3[enemies.Length];
            for (int i = 0; i < enemies.Length; i++)
                initialPositions[i] = enemies[i] != null ? enemies[i].transform.position : Vector3.zero;
            ActivateRoom(startActive);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) ActivateRoom(true);
        }

        public void ActivateRoom(bool status)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                var enemy = enemies[i];
                if (enemy == null) continue;

                // Killed enemies stay dead, otherwise re-entering the room would undo the fight.
                if (status && enemy.TryGetComponent(out Health health) && !health.IsAlive) continue;

                enemy.SetActive(status);
                if (status) enemy.transform.position = initialPositions[i];
            }
        }
    }
}
