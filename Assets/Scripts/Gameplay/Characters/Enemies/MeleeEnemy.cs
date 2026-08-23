using UnityEngine;
using Core.Constants;
using Core.Physics;
using Core.State;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    public sealed class MeleeEnemy : EnemyBase
    {
        float cooldownTimer;
        Health playerHealth;
        float attackRangeSqr;
        Rigidbody2D body;
        bool chasing;

        protected override void Awake()
        {
            base.Awake();
            body = KinematicBody.Prepare(this);
            if (config != null) attackRangeSqr = config.attackRange * config.attackRange;
        }

        void Update()
        {
            if (IsDead || !GameStateManager.IsCurrentlyPlaying) return;
            if (!TryResolveTarget()) return;
            if (playerHealth == null && playerTransform != null) playerTransform.TryGetComponent(out playerHealth);
            cooldownTimer += Time.deltaTime;

            if (!PlayerVisible || !InPatrolBounds())
            {
                SetPatrol(true);
                chasing = false;
                return;
            }

            SetPatrol(false);
            chasing = true;
            if (cooldownTimer >= config.attackCooldown && InAttackRange()) Attack();
        }

        void FixedUpdate()
        {
            if (!chasing || IsDead || !GameStateManager.IsCurrentlyPlaying || playerTransform == null) return;
            float currentX = body != null ? body.position.x : transform.position.x;
            float dir = Mathf.Sign(playerTransform.position.x - currentX);
            float newX = currentX + dir * config.chaseSpeed * Time.fixedDeltaTime;
            if (patrol == null || (newX >= patrol.LeftEdge.position.x && newX <= patrol.RightEdge.position.x))
            {
                KinematicBody.MoveTo(body, transform, new(newX, transform.position.y, transform.position.z));
                transform.localScale = new(dir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                anim.SetBool(GameConstants.Anim.Moving, true);
            }
        }

        void Attack()
        {
            cooldownTimer = 0f;
            anim.SetTrigger(GameConstants.Anim.MeleeAttack);
            playerHealth?.TakeDamage(Damage);
        }

        bool InPatrolBounds() =>
            patrol == null || (playerTransform.position.x >= patrol.LeftEdge.position.x &&
                              playerTransform.position.x <= patrol.RightEdge.position.x);

        bool InAttackRange() =>
            (transform.position - playerTransform.position).sqrMagnitude <= attackRangeSqr;
    }
}
