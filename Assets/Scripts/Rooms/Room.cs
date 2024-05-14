using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    private Vector3[] initialPositions;

    private void Awake()
    {
        // Save the initial positions of the enemies
        initialPositions = new Vector3[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
                initialPositions[i] = enemies[i].transform.position;
        }

        // Deactivate rooms
        if (transform.GetSiblingIndex() != 0)
            ActivateRoom(false);
    }

    public void ActivateRoom(bool status)
    {
        // Activate/deactivate enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].SetActive(status);
                // Reset the enemy's position when reactivating
                if (status)
                    enemies[i].transform.position = initialPositions[i];
            }
        }
    }
}
