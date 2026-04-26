using UnityEngine;
using Core.State;
using Core.Constants;
using Core.Interfaces;

namespace Gameplay.Characters.Enemies
{
    public sealed class MeleeEnemy : EnemyBase
    {
        [Header("Combat")]
        [SerializeField] float attackCooldown = 1f;
        [SerializeField] float attackRange = 1.5f;
        [SerializeField] float chaseSpeed = 3f;
        
        [Header("Target (auto-finds if empty)")]
        [SerializeField] Transform playerTransform;

        float cooldownTimer;
        Player.Player player;
        IDamageable playerHealth;
        PatrolMovement patrol;

        protected override void Awake()
        {
            base.Awake();
            patrol = GetComponentInParent<PatrolMovement>();
            
            if (playerTransform == null) playerTransform = GameConstants.FindPlayer();
            if (playerTransform != null)
            {
                player = playerTransform.GetComponent<Player.Player>();
                playerTransform.TryGetComponent(out playerHealth);
            }
        }

        void Update()
        {
            if (isDead || !GameStateManager.IsCurrentlyPlaying || playerTransform == null) return;
            cooldownTimer += Time.deltaTime;

            if ((player == null || !player.IsInvisible) && InPatrolBounds())
            {
                if (patrol != null) patrol.enabled = false;
                ChasePlayer();
            if (cooldownTimer >= attackCooldown && InAttackRange()) Attack();
        }
        else if (patrol != null) patrol.enabled = true;
    }

    void Attack()
    {
        cooldownTimer = 0f;
        anim.SetTrigger(GameConstants.Animation.MeleeAttack);
        playerHealth?.TakeDamage(damage);
    }

    void ChasePlayer()
    {
        float dir = Mathf.Sign(playerTransform.position.x - transform.position.x);
        float newX = transform.position.x + dir * chaseSpeed * Time.deltaTime;
        
        if (patrol == null || (newX >= patrol.LeftEdge.position.x && newX <= patrol.RightEdge.position.x))
        {
            transform.position = new(newX, transform.position.y, transform.position.z);
            transform.localScale = new(dir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            anim.SetBool(GameConstants.Animation.Moving, true);
        }
    }

    bool InPatrolBounds() => patrol == null || (playerTransform.position.x >= patrol.LeftEdge.position.x && playerTransform.position.x <= patrol.RightEdge.position.x);
    bool InAttackRange() => Vector2.Distance(transform.position, playerTransform.position) <= attackRange;
}
}