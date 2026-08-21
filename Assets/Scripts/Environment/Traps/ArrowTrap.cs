using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Services;
using Core.State;
using Gameplay.Combat;
using Gameplay.Characters.Player;

namespace Environment.Traps
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class ArrowTrap : MonoBehaviour
    {
        [SerializeField] float attackCooldown = 1f;
        [SerializeField] Transform firePoint;
        [SerializeField] GameObject[] arrows;
        [SerializeField] AudioClip arrowSound;
        [SerializeField] Transform playerTransform;

        float cooldownTimer;
        Player player;
        int arrowIndex;

        void Awake()
        {
            if (playerTransform == null) playerTransform = GameConstants.FindPlayer();
            player = playerTransform?.GetComponent<Player>();
        }

        void Update()
        {
            if (!GameStateManager.IsCurrentlyPlaying) return;
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer < attackCooldown) return;
            if (player == null && playerTransform != null) player = playerTransform.GetComponent<Player>();
            if (player is not { IsInvisible: false }) return;
            Attack();
        }

        void Attack()
        {
            cooldownTimer = 0;
            ServiceLocator.Get<IAudioManager>()?.PlaySound(arrowSound);
            ProjectileBase.Fire(arrows, ref arrowIndex, firePoint.position)?.ActivateProjectile();
        }
    }
}
