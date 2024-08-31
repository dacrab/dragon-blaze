using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float damage;

    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 3.0f;

    [Header("Collider Parameters")]
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Particle Systems")]
    [SerializeField] private GameObject hitParticleSystemPrefab;
    [SerializeField] private GameObject deathParticleSystemPrefab;

    [Header("Health Parameters")]
    [SerializeField] private float health = 100f;

    private float cooldownTimer = Mathf.Infinity;
    private Transform playerTransform;
    private Animator anim;
    private Health playerHealth;
    private EnemyPatrol enemyPatrol;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        InitializeComponents();
    }

    private void Update()
    {
        if (!ValidateComponents()) return;

        cooldownTimer += Time.deltaTime;

        if (PlayerInSight() && PlayerWithinPatrolBounds())
        {
            HandlePlayerDetected();
        }
        else
        {
            enemyPatrol.enabled = true;
        }
    }

    private void InitializeComponents()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerMovement = player.GetComponent<PlayerMovement>();
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

    private bool ValidateComponents()
    {
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component is not found!");
            return false;
        }

        if (enemyPatrol == null || playerTransform == null)
        {
            Debug.LogError("EnemyPatrol or PlayerTransform is null!");
            return false;
        }

        return true;
    }

    private void HandlePlayerDetected()
    {
        enemyPatrol.enabled = false;
        if (CanMoveForward())
        {
            FollowPlayer();
        }

        if (cooldownTimer >= attackCooldown)
        {
            Attack();
        }
    }

    private void Attack()
    {
        cooldownTimer = 0;
        anim.SetTrigger("meleeAttack");
        DamagePlayer();
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

        Collider2D hit = Physics2D.OverlapBox(checkPosition, boxCollider.size, 0, LayerMask.GetMask("Default"));
        return hit == null;
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
                MoveTowardsPlayer(proposedXPosition, direction);
            }
        }
        else
        {
            Debug.LogError("Patrol edges are not set or enemyPatrol is null!");
        }
    }

    private void MoveTowardsPlayer(float proposedXPosition, Vector3 direction)
    {
        transform.position = new Vector3(proposedXPosition, transform.position.y, transform.position.z);
        anim.SetBool("moving", true);

        if (direction.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void DamagePlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
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
        GetComponent<BoxCollider2D>().enabled = false;
        Instantiate(deathParticleSystemPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
