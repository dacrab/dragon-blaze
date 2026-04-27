using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;
using Gameplay.Combat;

namespace Environment.Traps
{
    public sealed class ArrowTrap : MonoBehaviour
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
        Characters.Player.Player player;
        int arrowIndex;

        void Awake()
        {
            if (playerTransform == null) playerTransform = GameConstants.FindPlayer();
            player = playerTransform?.GetComponent<Characters.Player.Player>();
        }

        void Update()
        {
            if (!GameStateManager.IsCurrentlyPlaying) return;
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown && (player == null || !player.IsInvisible)) Attack();
        }

        void Attack()
    {
        cooldownTimer = 0;
        GameManager.Instance?.PlaySound(arrowSound);
        
        if (arrows is not { Length: > 0 }) return;
        var arrow = arrows[arrowIndex];
        arrowIndex = (arrowIndex + 1) % arrows.Length;
        arrow.transform.position = firePoint.position;
        if (arrow.TryGetComponent<ProjectileBase>(out var proj)) proj.ActivateProjectile();
    }
}
}