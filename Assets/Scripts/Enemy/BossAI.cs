using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2.0f;
    public float attackRange = 5.0f;
    public float attackCooldown = 2.0f;
    public int health = 100;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float cooldownTimer = Mathf.Infinity;

    void Update()
    {
        MoveTowardsPlayer();
        if (Vector3.Distance(transform.position, player.position) <= attackRange && cooldownTimer >= attackCooldown)
        {
            Attack();
            cooldownTimer = 0;
        }
        cooldownTimer += Time.deltaTime;
    }

    private void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void Attack()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add death logic here (e.g., play animation, destroy object)
        Destroy(gameObject);
    }
}
