using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] additionalObjects;

    private Vector3[] initialEnemyPositions;

    private void Awake()
    {
        // Save the initial positions of the enemies
        initialEnemyPositions = new Vector3[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
                initialEnemyPositions[i] = enemies[i].transform.position;
        }

        // Deactivate the room if it's not the first room
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
                enemies[i].transform.position = status ? initialEnemyPositions[i] : Vector3.zero; // Reset enemy position if activating, move to zero if deactivating
            }
        }

        // Activate/deactivate additional objects
        for (int i = 0; i < additionalObjects.Length; i++)
        {
            if (additionalObjects[i] != null)
            {
                additionalObjects[i].SetActive(status);
            }
        }
    }
}
