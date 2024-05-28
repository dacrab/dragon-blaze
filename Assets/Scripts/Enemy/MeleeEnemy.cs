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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerMovement = player.GetComponent<PlayerMovement>();  // Ensure this line is correctly fetching the component
            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement component not found on the Player object!");
            }
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
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component is not found!");
            return; // Early exit to prevent further null reference issues
        }

        if (enemyPatrol == null || playerTransform == null)
        {
            Debug.LogError("EnemyPatrol or PlayerTransform is null!");
            return; // Early exit
        }

        cooldownTimer += Time.deltaTime;

        if (PlayerInSight() && PlayerWithinPatrolBounds())
        {
            enemyPatrol.enabled = false; // Disable patrol when player is in sight and within bounds
            if (CanMoveForward())
            {
                FollowPlayer(); // Aggressively follow the player
            }

            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("meleeAttack");
                DamagePlayer(); // Attack the player
            }
        }
        else
        {
            enemyPatrol.enabled = true; // Enable patrol if the player is not in sight or out of bounds
        }
    }

    private bool PlayerInSight()
    {
        return !playerMovement.IsInvisible();
    }

    private bool PlayerWithinPatrolBounds()
    {
        if (enemyPatrol.LeftEdge == null || enemyPatrol.RightEdge == null)
        {
            Debug.LogError("Patrol bounds (LeftEdge or RightEdge) are null!");
            return false;
        }
        return playerTransform.position.x >= enemyPatrol.LeftEdge.position.x &&
               playerTransform.position.x <= enemyPatrol.RightEdge.position.x;
    }

    private bool CanMoveForward()
    {
        Vector2 direction = transform.right * transform.localScale.x;
        Vector2 checkPosition = (Vector2)transform.position + (direction * boxCollider.size.x);

        // Check for ground and obstacles in front of the enemy
        Collider2D hit = Physics2D.OverlapBox(checkPosition, boxCollider.size, 0, LayerMask.GetMask("Default"));
        return hit == null; // If nothing is hit, it's safe to move forward
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

