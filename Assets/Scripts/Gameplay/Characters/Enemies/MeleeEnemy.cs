using UnityEngine;
using Core.State;
using Core.Constants;

namespace Gameplay.Characters.Enemies;

public sealed class MeleeEnemy : EnemyBase
{
    [SerializeField] float attackCooldown = 1f, attackRange = 1.5f, chaseSpeed = 3f;

    float cooldownTimer;
    Transform playerTransform;
    Player.Player player;
    EnemyPatrol patrol;

    protected override void Awake()
    {
        base.Awake();
        patrol = GetComponentInParent<EnemyPatrol>();
        var go = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
        if (go != null)
        {
            playerTransform = go.transform;
            player = go.GetComponent<Player.Player>();
        }
    }

    void Update()
    {
        if (isDead || !GameStateManager.IsCurrentlyPlaying || playerTransform == null) return;
        cooldownTimer += Time.deltaTime;

        if (player is { IsInvisible: false } && InPatrolBounds())
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
        playerTransform.GetComponent<Health.Health>()?.TakeDamage(damage);
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
