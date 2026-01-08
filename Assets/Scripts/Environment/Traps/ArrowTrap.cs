using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;
using Gameplay.Combat;

namespace Environment.Traps
{
    public class ArrowTrap : TrapBase
    {
        [SerializeField] float attackCooldown = 1f;
        [SerializeField] Transform firePoint;
        [SerializeField] GameObject[] arrows;
        [SerializeField] AudioClip arrowSound;

        float cooldownTimer;
        Gameplay.Characters.Player.Player player;
        int arrowIndex;

        void Awake()
        {
            var go = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (go != null) player = go.GetComponent<Gameplay.Characters.Player.Player>();
        }

        void Update()
        {
            if (!GameStateManager.IsCurrentlyPlaying) return;
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown && player is { IsInvisible: false }) Attack();
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
            arrow.GetComponent<EnemyProjectile>()?.ActivateProjectile();
        }
    }
}
