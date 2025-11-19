using UnityEngine;
using Core.Constants;
using Core.Systems;

public class RangedEnemy : EnemyBase
{
    #region Serialized Fields
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    // Damage inherited from Base

    [Header("Ranged Attack")]
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject[] fireballs; // Could use ObjectPool

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    // BoxCollider2D inherited as 'col'

    [Header("Player Detection")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Audio")]
    [SerializeField] private AudioClip fireballSound;
    #endregion

    #region Private Fields
    private float cooldownTimer = Mathf.Infinity;
    private EnemyPatrol enemyPatrol;
    private Player.PlayerController playerController; // Direct dependency on new system
    private Transform playerTransform;
    #endregion

    #region Unity Lifecycle Methods
    protected override void Awake()
    {
        base.Awake(); // Initializes anim, rb, col
        InitializeComponents();
    }

    private void Update()
    {
        if (isDead) return;

        UpdateCooldownTimer();
        
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("rangedAttack"); // Verify trigger name in Animator
            }
            
            if (enemyPatrol != null) enemyPatrol.enabled = false;
        }
        else
        {
            if (enemyPatrol != null) enemyPatrol.enabled = true;
        }
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        
        GameObject player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
        if (player != null)
        {
            playerTransform = player.transform;
            // Senior Dev move: Depend on the abstraction/interface or the new Controller
            playerController = player.GetComponent<Player.PlayerController>();
            
            // Fallback for migration phase if they haven't swapped yet
            if (playerController == null)
            {
                // This handles the case where user still has PlayerMovement script
                // We can try to get PlayerMovement and cast/use? 
                // But user said "remove legacy". So we enforce PlayerController.
                Debug.LogWarning("RangedEnemy: PlayerController not found on Player! Ensure Player has been migrated.");
            }
        }
    }
    #endregion

    #region Update Methods
    private void UpdateCooldownTimer()
    {
        cooldownTimer += Time.deltaTime;
    }
    #endregion

    #region Attack Methods
    // Called by Animation Event
    private void RangedAttack()
    {
        SoundManager.instance.PlaySound(fireballSound);
        cooldownTimer = 0;
        
        // Use Object Pool if available, else fallback to array
        // For now, sticking to array logic to minimize breaking changes in Inspector (assigning fireballs)
        // But refining the search.
        
        GameObject fireball = GetFireball();
        if (fireball != null)
        {
            fireball.transform.position = firepoint.position;
            // ActivateProjectile on EnemyProjectile handles direction/activation
            fireball.GetComponent<EnemyProjectile>().ActivateProjectile(); 
        }
    }

    private GameObject GetFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return fireballs[i];
        }
        return null;
    }
    #endregion

    #region Player Detection Methods
    private bool PlayerInSight()
    {
        if (playerController == null || playerController.IsInvisible()) return false;

        // Using inherited 'col' (needs cast if we want Bounds specifically from BoxCollider2D)
        BoxCollider2D box = col as BoxCollider2D;
        if (box == null) return false;

        RaycastHit2D hit = Physics2D.BoxCast(
            box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(box.bounds.size.x * range, box.bounds.size.y, box.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        return hit.collider != null && hit.collider.CompareTag(GameConstants.Tags.Player);
    }
    #endregion

    #region Gizmo Methods
    private void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(box.bounds.size.x * range, box.bounds.size.y, box.bounds.size.z));
    }
    #endregion
}