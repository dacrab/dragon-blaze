using UnityEngine;
using Core.State;
using Core.Constants;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    public sealed class MeleeEnemy : EnemyBase
    {
        [Header("Target")]
        [SerializeField] Transform playerTransform;

        float cooldownTimer;
        Player.Player player;
        Health playerHealth;
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
            if (IsDead || !GameStateManager.IsCurrentlyPlaying || playerTransform == null) return;
            cooldownTimer += Time.deltaTime;

            if ((player == null || !player.IsInvisible) && InPatrolBounds())
            {
                patrol.enabled = false;
                ChasePlayer();
                if (cooldownTimer >= config.attackCooldown && InAttackRange()) Attack();
            }
            else patrol.enabled = true;
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
            Vector2.Distance(transform.position, playerTransform.position) <= config.attackRange;
    }
}
