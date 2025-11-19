using UnityEngine;

public class Room : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private GameObject[] enemies;
    #endregion

    #region Private Fields
    private Vector3[] initialPositions;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        SaveInitialEnemyPositions();
        DeactivateRoomIfNotFirst();
    }
    #endregion

    #region Public Methods
    public void ActivateRoom(bool status)
    {
        ToggleEnemies(status);
    }
    #endregion

    #region Private Methods
    private void SaveInitialEnemyPositions()
    {
        initialPositions = new Vector3[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
                initialPositions[i] = enemies[i].transform.position;
        }
    }

    private void DeactivateRoomIfNotFirst()
    {
        if (transform.GetSiblingIndex() != 0)
            ActivateRoom(false);
    }

    private void ToggleEnemies(bool status)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].SetActive(status);
                if (status)
                    ResetEnemyPosition(i);
            }
        }
    }

    private void ResetEnemyPosition(int index)
    {
        enemies[index].transform.position = initialPositions[index];
    }
    #endregion
}
