using UnityEngine;

namespace Environment.Rooms
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;
        private Vector3[] initialPositions;

        private void Awake()
        {
            initialPositions = new Vector3[enemies.Length];
            for (int i = 0; i < enemies.Length; i++)
                if (enemies[i] != null) initialPositions[i] = enemies[i].transform.position;
            
            if (transform.GetSiblingIndex() != 0) ActivateRoom(false);
        }

        public void ActivateRoom(bool status)
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
