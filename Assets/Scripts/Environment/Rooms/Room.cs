using UnityEngine;

namespace Environment.Rooms
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;

        private Vector3[] initialPositions;

        private void Awake()
        {
            CacheInitialPositions();
            if (transform.GetSiblingIndex() != 0)
                SetActive(false);
        }

        public void SetActive(bool active)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null) continue;
                
                enemies[i].SetActive(active);
                if (active)
                    enemies[i].transform.position = initialPositions[i];
            }
        }

        public void ActivateRoom(bool status) => SetActive(status);

        private void CacheInitialPositions()
        {
            initialPositions = new Vector3[enemies.Length];
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                    initialPositions[i] = enemies[i].transform.position;
            }
        }
    }
}
