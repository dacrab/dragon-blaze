using UnityEngine;
using Core.Constants;

// Original inherited from EnemyDamage. 
// EnemyDamage logic: OnTriggerEnter -> Check Player -> damage.
// ProjectileBase logic: OnTriggerEnter -> hit=true -> anim/deactivate.
// We need to combine them. EnemyProjectile should likely inherit ProjectileBase for movement/lifecycle,
// and override OnTriggerEnter to damage Player.
// Note: EnemyDamage is basically a script that says "I hurt player on touch".
// ProjectileBase is "I fly and die". 
// EnemyProjectile is both.

public class EnemyProjectile : ProjectileBase
{
    // Inherits speed, damage, lifetime from ProjectileBase
    
    // Original had "resetTime" instead of "lifetime". Mapping to maxLifetime.
    
    protected override void Awake()
    {
        base.Awake();
        // Map resetTime to maxLifetime if we want to keep inspector value or just rename in Unity (safer to keep field and map it)
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // Filter collision - only hit Player or Ground?
        // Original logic: "ShouldProcessCollision" -> CompareTag("Player") or !Visible
        
        if (!collision.CompareTag(GameConstants.Tags.Player)) 
        {
            // If hit ground, destroy/explode? 
            // Base logic just says "hit anything".
            // Original code: OnTriggerEnter2D -> HandleCollision -> hit=true, anim.
            // But HandleCollision was called ONLY if ShouldProcessCollision returned true.
            // ShouldProcessCollision returned true IF tag != Player (so ground/walls?) OR (Player && Visible).
            
            // So it hits everything, but only damages Player?
            // Actually wait. ShouldProcessCollision:
            // if (!collision.CompareTag("Player")) return true; (Process non-players)
            // PlayerMovement p = ...; return p==null || Visible; (Process visible players)
            
            base.OnTriggerEnter2D(collision);
            return;
        }

        // It is player
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null && player.IsVisible())
        {
             Health playerHealth = collision.GetComponent<Health>();
             if (playerHealth) playerHealth.TakeDamage(damage);
             
             base.OnTriggerEnter2D(collision);
        }
    }

    public void ActivateProjectile()
    {
        // Adapter for old method name
        // Assuming direction is set via transform rotation or separate call?
        // Original code didn't take direction in Activate(), it moved Translate(speed * dt, 0, 0)
        // So it relied on rotation being set by shooter.
        
        // ProjectileBase ResetProjectile takes direction. 
        // If direction is 0, it won't move in X.
        // But ProjectileBase moves via Translate(speed * dt * direction).
        // If direction is used for flipping scale, we need to be careful.
        
        // EnemyProjectile original used: Translate(speed*dt, 0, 0). No direction var.
        // This means it moves in its local Right always.
        
        // ProjectileBase Move(): Translate(speed * dt * direction, 0, 0).
        // If we set direction = 1, it behaves like original.
        
        SetDirection(1); // Default to local forward
    }
}