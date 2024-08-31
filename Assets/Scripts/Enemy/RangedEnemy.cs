using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    #region Serialized Fields
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;

    [Header("Ranged Attack")]
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject[] fireballs;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Audio")]
    [SerializeField] private AudioClip fireballSound;
    #endregion

    #region Private Fields
    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private EnemyPatrol enemyPatrol;
    private PlayerMovement playerMovement;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        InitializeComponents();
    }

    private void Update()
    {
        UpdateCooldownTimer();
        HandleAttack();
        UpdateEnemyPatrol();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
        else
        {
            Debug.LogError("Player object not found with tag 'Player'!");
        }
    }
    #endregion

    #region Update Methods
    private void UpdateCooldownTimer()
    {
        cooldownTimer += Time.deltaTime;
    }

    private void HandleAttack()
    {
        if (PlayerInSight() && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0;
            anim.SetTrigger("rangedAttack");
        }
    }

    private void UpdateEnemyPatrol()
    {
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }
    #endregion

    #region Attack Methods
    private void RangedAttack()
    {
        SoundManager.instance.PlaySound(fireballSound);
        cooldownTimer = 0;
        int fireballIndex = FindFireball();
        fireballs[fireballIndex].transform.position = firepoint.position;
        fireballs[fireballIndex].GetComponent<EnemyProjectile>().ActivateProjectile();
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
    #endregion

    #region Player Detection Methods
    private bool PlayerInSight()
    {
        if (playerMovement.IsInvisible()) return false;

        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }
    #endregion

    #region Gizmo Methods
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
    #endregion
}