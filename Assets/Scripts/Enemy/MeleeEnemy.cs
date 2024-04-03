using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown; // Cooldown time between attacks
    [SerializeField] private float range; // Attack range
    [SerializeField] private int damage; // Amount of damage inflicted

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance; // Distance of the attack collider from the enemy
    [SerializeField] private BoxCollider2D boxCollider; // Reference to the attack collider

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; // Layer mask for detecting the player

    private float cooldownTimer = Mathf.Infinity; // Timer for tracking attack cooldown

    // References
    private Animator anim; // Animator component of the enemy
    private Health playerHealth; // Player's health component
    private EnemyPatrol enemyPatrol; // Enemy patrol script
    private PlayerMovement playerMovement; // Reference to PlayerMovement script

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        playerMovement = FindObjectOfType<PlayerMovement>(); // Finding PlayerMovement script
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Attack only when player in sight and visible
        if (PlayerInSight() && playerMovement.IsVisible()) // Check if the player is visible
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("meleeAttack");
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    // Check if the player is within attack range
    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
            playerHealth = hit.transform.GetComponent<Health>();

        return hit.collider != null;
    }

    // Draw attack range Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    // Inflict damage to the player
    private void DamagePlayer()
    {
        if (PlayerInSight() && playerMovement.IsVisible()) // Check if the player is visible
            playerHealth.TakeDamage(damage);
    }
}
