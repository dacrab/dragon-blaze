using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Services;
using Core.State;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    using Player = Gameplay.Characters.Player.Player;

    public sealed class RangedEnemy : EnemyBase
    {
        [Header("Projectiles")]
        [SerializeField] Transform firepoint;
        [SerializeField] GameObject[] fireballs;

        [Header("Detection")]
        [SerializeField] LayerMask playerLayer;

        [Header("Target")]
        [SerializeField] Transform playerTransform;

        [Header("Sight")]
        [SerializeField] float detectionInterval = 0.2f;

        float cooldownTimer, detectionTimer;
        bool playerInSight;
        PatrolMovement patrol;
        Player player;
        int fireballIndex;

        protected override void Awake()
        {
            base.Awake();
            patrol = GetComponentInParent<PatrolMovement>();
            if (playerTransform == null) playerTransform = GameConstants.FindPlayer();
            player = playerTransform?.GetComponent<Player>();
        }

        void Update()
        {
            if (IsDead || !GameStateManager.IsCurrentlyPlaying) return;
            cooldownTimer += Time.deltaTime;

            if (playerTransform == null) playerTransform = GameConstants.FindPlayer();
            if (player == null && playerTransform != null) player = playerTransform.GetComponent<Player>();

            if (player is { IsInvisible: true })
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

        void SetPatrol(bool enabled)
        {
            if (patrol != null) patrol.enabled = enabled;
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
