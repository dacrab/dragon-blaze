using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;
using Core.Interfaces;
using Gameplay.Combat;

namespace Environment.Traps;

public sealed class ArrowTrap : TrapBase
{
    [Header("Combat")]
    [SerializeField] float attackCooldown = 1f;
    
    [Header("Projectiles")]
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject[] arrows;
    
    [Header("Audio")]
    [SerializeField] AudioClip arrowSound;
    
    [Header("Target (auto-finds if empty)")]
    [SerializeField] Transform playerTransform;

    float cooldownTimer;
    IInvisible playerInvisible;
    int arrowIndex;

    void Awake()
    {
        if (playerTransform == null)
        {
            var go = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (go != null) playerTransform = go.transform;
        }
        
        playerTransform?.TryGetComponent(out playerInvisible);
    }

    void Update()
    {
        if (!GameStateManager.IsCurrentlyPlaying) return;
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= attackCooldown && playerInvisible is not { IsInvisible: true }) Attack();
    }

    protected override void OnTriggerEnter2D(Collider2D collision) { }

    void Attack()
    {
        cooldownTimer = 0;
        SoundManager.Instance?.PlaySound(arrowSound);
        
        if (arrows is not { Length: > 0 }) return;
        var arrow = arrows[arrowIndex];
        arrowIndex = (arrowIndex + 1) % arrows.Length;
        arrow.transform.position = firePoint.position;
        if (arrow.TryGetComponent<EnemyProjectile>(out var proj)) proj.ActivateProjectile();
    }
}
