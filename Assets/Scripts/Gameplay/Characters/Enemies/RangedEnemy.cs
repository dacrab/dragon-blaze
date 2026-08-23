using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Services;
using Core.State;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    public sealed class RangedEnemy : EnemyBase
    {
        [Header("Projectiles")]
        [SerializeField] Transform firepoint;
        [SerializeField] GameObject[] fireballs;

        [Header("Detection")]
        [SerializeField] LayerMask playerLayer;

        [Header("Sight")]
        [SerializeField] float detectionInterval = 0.2f;

        float cooldownTimer, detectionTimer;
        bool playerInSight;
        int fireballIndex;

        void Update()
        {
            if (IsDead || !GameStateManager.IsCurrentlyPlaying) return;
            cooldownTimer += Time.deltaTime;

            if (!TryResolveTarget()) return;

            if (!PlayerVisible)
            {
                playerInSight = false;
                SetPatrol(true);
                return;
            }

            detectionTimer += Time.deltaTime;
            if (detectionTimer >= detectionInterval)
            {
                detectionTimer = 0;
                playerInSight = PlayerInSight();
            }

            SetPatrol(!playerInSight);
            if (playerInSight && cooldownTimer >= config.attackCooldown)
            {
                cooldownTimer = 0f;
                anim.SetTrigger(GameConstants.Anim.RangedAttack);
            }
        }

        void RangedAttack()
        {
            ServiceLocator.Get<IAudioManager>()?.PlaySound(config.attackSound);
            ProjectileBase.Fire(fireballs, ref fireballIndex, firepoint.position)?.ActivateProjectile();
        }

        bool PlayerInSight()
        {
            var bounds = col.bounds;
            var hit = Physics2D.BoxCast(
                bounds.center + transform.right * config.detectionRange * 0.5f * transform.localScale.x,
                new Vector3(config.detectionRange, bounds.size.y, 1f), 0, Vector2.zero, 0, playerLayer);
            return hit.collider != null;
        }
    }
}
