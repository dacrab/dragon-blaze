using UnityEngine;

public class EnemyFireballHolder : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform enemy;
    #endregion

    #region Unity Lifecycle Methods
    private void Update()
    {
        UpdateScale();
    }
    #endregion

    #region Scale Management
    private void UpdateScale()
    {
        transform.localScale = enemy.localScale;
    }
    #endregion
}