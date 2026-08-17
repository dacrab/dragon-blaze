using UnityEngine;
using Core.Constants;
using Core.Managers;
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
            if (playerTransform != null) player = playerTransform.GetComponent<Player>();
        }

        void Update()
        {
            if (!GameStateManager.IsCurrentlyPlaying) return;
            if (player == null || playerTransform == null)
            {
                playerTransform = GameConstants.FindPlayer();
                player = playerTransform?.GetComponent<Player>();
                if (player == null) return;
            }
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown && !player.IsInvisible) Attack();
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
