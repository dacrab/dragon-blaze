using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown; // Cooldown time between attacks
    [SerializeField] private float range; // Attack range
    [SerializeField] private float damage; // Amount of damage inflicted as a float

    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 3.0f; // Speed at which the enemy moves towards the player

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance; // Distance of the attack collider from the enemy
    [SerializeField] private BoxCollider2D boxCollider; // Reference to the attack collider

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; // Layer mask for detecting the player

    [SerializeField] private GameObject hitParticleSystemPrefab; // Particle system prefab for player hit effects
    [SerializeField] private GameObject deathParticleSystemPrefab; // Particle system prefab for enemy death effects

    [Header("Health Parameters")]
    [SerializeField] private float health = 100f; // Enemy's health

    private float cooldownTimer = Mathf.Infinity; // Timer for tracking attack cooldown
    private Transform playerTransform; // Player's transform

    // References
    private Animator anim; // Animator component of the enemy
    private Health playerHealth; // Player's health component
    private EnemyPatrol enemyPatrol; // Enemy patrol script
    private PlayerMovement playerMovement; // Reference to PlayerMovement script

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object not found with tag 'Player'!");
        }

        if (enemyPatrol == null)
        {
            Debug.LogError("EnemyPatrol component not found on the parent object!");
        }
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            FollowPlayer(); // Follow the player when in sight
        }
        else
        {
            if (enemyPatrol != null)
                enemyPatrol.enabled = true; // Enable patrol if the player is not in sight
        }
    }

    private void FollowPlayer()
    {
        if (playerTransform == null)
        {
            Debug.LogError("playerTransform is null");
            return;
        }
        if (!PlayerInSight())
        {
            Debug.LogError("Player not in sight");
            return;
        }

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float proposedXPosition = transform.position.x + direction.x * moveSpeed * Time.deltaTime;

        if (enemyPatrol != null && enemyPatrol.LeftEdge != null && enemyPatrol.RightEdge != null)
        {
            if (proposedXPosition >= enemyPatrol.LeftEdge.position.x && proposedXPosition <= enemyPatrol.RightEdge.position.x)
            {
                transform.position = new Vector3(proposedXPosition, transform.position.y, transform.position.z);
                anim.SetBool("moving", true);

                if (direction.x > 0)
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                else if (direction.x < 0)
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            Debug.LogError("Patrol edges are not set or enemyPatrol is null!");
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("meleeAttack");
            }
            return true;
        }
        return false;
    }

    public void OnMeleeAttack()
    {
        if (PlayerInSight()) // Check again if player is in sight when actually attacking
            DamagePlayer();
    }

    private void DamagePlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);

            // Play particle system at player's position for hit effects
            Instantiate(hitParticleSystemPrefab, playerTransform.position, Quaternion.identity);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Disable the BoxCollider2D component to prevent further interactions
        GetComponent<BoxCollider2D>().enabled = false;

        // Play particle system at enemy's position for death effects
        Instantiate(deathParticleSystemPrefab, transform.position, Quaternion.identity);

        // Other death-related logic
        // For example, you might want to destroy the game object after some delay
        Destroy(gameObject);
    }
}
