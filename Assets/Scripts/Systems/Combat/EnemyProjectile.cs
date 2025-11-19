using UnityEngine;
using Core.Constants;
using Player; // Added

public class EnemyProjectile : ProjectileBase
{
    // Inherits speed, damage, lifetime from ProjectileBase
    
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.Tags.Player)) 
        {
            base.OnTriggerEnter2D(collision);
            return;
        }

        // It is player
        // Use PlayerController to check visibility
        PlayerController player = collision.GetComponent<PlayerController>();
        
        // If PlayerController is missing (migration issue) or Visible
        // But we removed PlayerMovement, so we rely on PlayerController.
        if (player != null && !player.IsInvisible())
        {
             Health playerHealth = collision.GetComponent<Health>();
             if (playerHealth) playerHealth.TakeDamage(damage);
             
             base.OnTriggerEnter2D(collision);
        }
    }

    public void ActivateProjectile()
    {
        SetDirection(1); // Default to local forward
    }
}