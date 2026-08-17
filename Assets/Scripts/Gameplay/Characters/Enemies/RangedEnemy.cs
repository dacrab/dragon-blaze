using UnityEngine;
using Core.Managers;
using Core.Constants;
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

        [Header("Target")]
        [SerializeField] Transform playerTransform;

        float cooldownTimer;
        PatrolMovement patrol;
        Player.Player player;
        int fireballIndex;

        protected override void Awake()
        {
            base.Awake();
            patrol = GetComponentInParent<PatrolMovement>();
            if (playerTransform == null) playerTransform = GameConstants.FindPlayer();
            player = playerTransform?.GetComponent<Player.Player>();
        }

        void Update()
        {
            if (IsDead || !GameStateManager.IsCurrentlyPlaying) return;
            cooldownTimer += Time.deltaTime;

            if (PlayerInSight())
            {
                if (patrol != null) patrol.enabled = false;
                if (cooldownTimer >= config.attackCooldown)
                {
                    cooldownTimer = 0f;
                    anim.SetTrigger(GameConstants.Anim.RangedAttack);
                }
            }
            else if (patrol != null) patrol.enabled = true;
        }

        void RangedAttack()
        {
            GameManager.Instance?.PlaySound(config.attackSound);
            if (fireballs is not { Length: > 0 }) return;
            var fb = fireballs[fireballIndex];
            fireballIndex = (fireballIndex + 1) % fireballs.Length;
            fb.transform.position = firepoint.position;
            if (fb.TryGetComponent<ProjectileBase>(out var proj)) proj.ActivateProjectile();
        }

        bool PlayerInSight()
        {
            if (player != null && player.IsInvisible) return false;
            var bounds = col.bounds;
            var hit = Physics2D.BoxCast(
                bounds.center + transform.right * config.detectionRange * 0.5f * transform.localScale.x,
                new Vector3(config.detectionRange, bounds.size.y, 1f), 0, Vector2.zero, 0, playerLayer);
            return hit.collider != null;
        }
    }
}
