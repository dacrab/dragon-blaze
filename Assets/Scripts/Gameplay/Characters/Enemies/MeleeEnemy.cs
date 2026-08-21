using UnityEngine;
using Core.Constants;
using Core.State;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    using Player = Gameplay.Characters.Player.Player;

    public sealed class MeleeEnemy : EnemyBase
    {
        [Header("Target")]
        [SerializeField] Transform playerTransform;

        float cooldownTimer;
        Player player;
        Health playerHealth;
        PatrolMovement patrol;
        float attackRangeSqr;

        protected override void Awake()
        {
            base.Awake();
            patrol = GetComponentInParent<PatrolMovement>();
            if (config != null) attackRangeSqr = config.attackRange * config.attackRange;
        }

        void Update()
        {
            if (IsDead || !GameStateManager.IsCurrentlyPlaying) return;
            if (playerTransform == null)
            {
                playerTransform = GameConstants.FindPlayer();
                if (playerTransform == null) return;
                player = playerTransform.GetComponent<Player>();
                playerTransform.TryGetComponent(out playerHealth);
            }
            cooldownTimer += Time.deltaTime;

            bool playerVisible = player == null || !player.IsInvisible;
            if (!playerVisible || !InPatrolBounds())
            {
                SetPatrol(true);
                return;
            }

            SetPatrol(false);
            ChasePlayer();
            if (cooldownTimer >= config.attackCooldown && InAttackRange()) Attack();
        }

        void SetPatrol(bool enabled)
        {
            if (patrol != null) patrol.enabled = enabled;
        }

        void Attack()
        {
            cooldownTimer = 0f;
            anim.SetTrigger(GameConstants.Anim.MeleeAttack);
            playerHealth?.TakeDamage(Damage);
        }

        void ChasePlayer()
        {
            float dir = Mathf.Sign(playerTransform.position.x - transform.position.x);
            float newX = transform.position.x + dir * config.chaseSpeed * Time.deltaTime;
            if (patrol == null || (newX >= patrol.LeftEdge.position.x && newX <= patrol.RightEdge.position.x))
            {
                transform.position = new(newX, transform.position.y, transform.position.z);
                transform.localScale = new(dir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                anim.SetBool(GameConstants.Anim.Moving, true);
            }
        }

        bool InPatrolBounds() =>
            patrol == null || (playerTransform.position.x >= patrol.LeftEdge.position.x &&
                              playerTransform.position.x <= patrol.RightEdge.position.x);

        bool InAttackRange() =>
            (transform.position - playerTransform.position).sqrMagnitude <= attackRangeSqr;
    }
}
