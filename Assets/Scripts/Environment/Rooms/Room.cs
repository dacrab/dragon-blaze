using UnityEngine;

namespace Environment.Rooms;

public sealed class Room : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    Vector3[] initialPositions;

    void Awake()
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
