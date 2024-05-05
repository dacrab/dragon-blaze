using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
 
    [Header("Scale Flip Parameters")]
    [SerializeField] private float flipSpeed; // Speed of the scale flip

    private Transform playerTransform; // Reference to the player's transform
    private bool playerAbove; // Flag to indicate if the player is above the enemy
    private bool playerBehind; // Flag to indicate if the player is behind the enemy

    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown; // Cooldown time between attacks
    [SerializeField] private float range; // Attack range
    [SerializeField] private int damage; // Amount of damage inflicted

    [Header("Ranged Attack")]
    [SerializeField] private Transform firepoint; // Point where the fireball is instantiated
    [SerializeField] private GameObject[] fireballs; // Array of fireball prefabs

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance; // Distance of the attack collider from the enemy
    [SerializeField] private BoxCollider2D boxCollider; // Reference to the attack collider

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; // Layer mask for detecting the player

    [Header("Fireball Sound")]
    [SerializeField] private AudioClip fireballSound; // Sound played when firing a fireball

    private float cooldownTimer = Mathf.Infinity; // Timer for tracking attack cooldown

    // References
    private Animator anim; // Animator component of the enemy
    private EnemyPatrol enemyPatrol; // Enemy patrol script
    private PlayerMovement playerMovement; // Reference to PlayerMovement script

    private void Awake()
    {
        // Get the player's transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>(); // Find and cache PlayerMovement script
    }

private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Check if player is above or behind the enemy
        playerAbove = playerTransform.position.y > transform.position.y;
        playerBehind = playerTransform.position.x < transform.position.x;

        // Flip scale to face left or right based on player's position
        if (playerBehind)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // Face left
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // Face right
        }

        // Attack only when player in sight and player is visible
        if (PlayerInSight() && playerMovement.IsVisible()) // Check if player is visible using IsVisible method
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("rangedAttack");
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight() || !playerMovement.IsVisible(); // Disable patrol when player is invisible
    }

    // Perform the ranged attack
    private void RangedAttack()
    {
        SoundManager.instance.PlaySound(fireballSound); // Play fireball sound
        cooldownTimer = 0;
        fireballs[FindFireball()].transform.position = firepoint.position; // Position the fireball at the firepoint
        fireballs[FindFireball()].GetComponent<EnemyProjectile>().ActivateProjectile(); // Activate the fireball
    }

    // Find an inactive fireball from the array
    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    // Check if the player is within attack range
    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }
    // Draw attack range Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
}
