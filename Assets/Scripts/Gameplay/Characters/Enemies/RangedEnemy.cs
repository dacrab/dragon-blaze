using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    public sealed class RangedEnemy : EnemyBase
    {
        [Header("Combat")]
        [SerializeField] float attackCooldown = 1f;
        [SerializeField] float range = 10f;
        
        [Header("Projectiles")]
        [SerializeField] Transform firepoint;
        [SerializeField] GameObject[] fireballs;
        
        [Header("Detection")]
        [SerializeField] LayerMask playerLayer;
        
        [Header("Audio")]
        [SerializeField] AudioClip fireballSound;
        
        [Header("Target (auto-finds if empty)")]
        [SerializeField] Transform playerTransform;

        float cooldownTimer;
        EnemyPatrol patrol;
        Player.Player player;
        int fireballIndex;

        protected override void Awake()
        {
            base.Awake();
            patrol = GetComponentInParent<EnemyPatrol>();
            
            if (playerTransform == null) playerTransform = GameConstants.FindPlayer();
            player = playerTransform?.GetComponent<Player.Player>();
        }

        void Update()
        {
            if (isDead || !GameStateManager.IsCurrentlyPlaying) return;
            cooldownTimer += Time.deltaTime;

            if (PlayerInSight())
            {
                if (patrol != null) patrol.enabled = false;
                if (cooldownTimer >= attackCooldown)
                {
                    cooldownTimer = 0f;
                    anim.SetTrigger(GameConstants.Animation.RangedAttack);
            }
        }
        else if (patrol != null) patrol.enabled = true;
    }

    void RangedAttack()
    {
        SoundManager.Instance?.PlaySound(fireballSound);
        if (fireballs is not { Length: > 0 }) return;
        
        var fb = fireballs[fireballIndex];
        fireballIndex = (fireballIndex + 1) % fireballs.Length;
        fb.transform.position = firepoint.position;
        if (fb.TryGetComponent<ProjectileBase>(out var proj)) proj.ActivateProjectile();
    }

    bool PlayerInSight()
    {
        if (player != null && player.IsInvisible) return false;
        if (col is not BoxCollider2D box) return false;

        var hit = Physics2D.BoxCast(
            box.bounds.center + transform.right * range * 0.5f * transform.localScale.x,
            new Vector3(range, box.bounds.size.y, 1f), 0, Vector2.zero, 0, playerLayer);
        return hit.collider != null;
    }
}
}