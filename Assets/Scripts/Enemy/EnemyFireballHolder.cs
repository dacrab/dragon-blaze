using UnityEngine;

public class EnemyFireballHolder : MonoBehaviour
{
    [SerializeField] private Transform enemy; // Reference to the enemy object

    // Update is called once per frame
    private void Update()
    {
        // Set the scale of the fireball holder to match the scale of the enemy
        transform.localScale = enemy.localScale;
    }
}
