using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected float damage;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerMovement == null || !playerMovement.IsVisible()) return;

        Health playerHealth = collision.GetComponent<Health>();
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damage);
    }
}