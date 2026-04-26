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
    IInvisible playerInvisible;
    IDamageable playerHealth;
    EnemyPatrol patrol;

    protected override void Awake()
    {
        base.Awake();
        patrol = GetComponentInParent<EnemyPatrol>();
        
        if (playerTransform == null)
        {
            var go = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (go != null) playerTransform = go.transform;
        }
        
        if (playerTransform != null)
        {
            playerTransform.TryGetComponent(out playerInvisible);
            playerTransform.TryGetComponent(out playerHealth);
        }
    }

    void Update()
    {
        if (isDead || !GameStateManager.IsCurrentlyPlaying || playerTransform == null) return;
        cooldownTimer += Time.deltaTime;

        if (playerInvisible is not { IsInvisible: true } && InPatrolBounds())
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