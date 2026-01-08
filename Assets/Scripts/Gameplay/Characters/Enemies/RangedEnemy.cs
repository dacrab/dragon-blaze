using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies;

public sealed class RangedEnemy : EnemyBase
{
    [SerializeField] float attackCooldown = 1f, range = 10f;
    [SerializeField] Transform firepoint;
    [SerializeField] GameObject[] fireballs;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] AudioClip fireballSound;

    float cooldownTimer;
    EnemyPatrol patrol;
    Player.Player player;
    int fireballIndex;

    protected override void Awake()
    {
        base.Awake();
        patrol = GetComponentInParent<EnemyPatrol>();
        var go = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
        if (go != null) player = go.GetComponent<Player.Player>();
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
        fb.GetComponent<EnemyProjectile>()?.ActivateProjectile();
    }

    bool PlayerInSight()
    {
        if (player is not { IsInvisible: false }) return false;
        if (col is not BoxCollider2D box) return false;

        var hit = Physics2D.BoxCast(
            box.bounds.center + transform.right * range * 0.5f * transform.localScale.x,
            new Vector3(range, box.bounds.size.y, 1f), 0, Vector2.zero, 0, playerLayer);
        return hit.collider != null;
    }
}
