using UnityEngine;
using Core.Constants;

public abstract class TrapBase : MonoBehaviour
{
    [SerializeField] protected float damage;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tags.Player))
        {
             DealDamage(collision.gameObject);
        }
    }

    protected virtual void DealDamage(GameObject target)
    {
        // Try to get Health from Player
        Health playerHealth = target.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        
        // Or maybe generic health?
        // In this project, Health.cs is seemingly generic or for player?
        // EnemyBase has health too. 
    }
}
