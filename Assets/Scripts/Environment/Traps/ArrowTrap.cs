using UnityEngine;
using Core.Managers;
using Core.Utilities;
using Gameplay.Combat;
using Core.Optimization;

namespace Environment.Traps
{
    public class ArrowTrap : TrapBase
    {
        [SerializeField] private float attackCooldown;
        [SerializeField] private Transform firePoint;
        [SerializeField] private string arrowTag = "Arrow";

        [Header("SFX")]
        [SerializeField] private AudioClip arrowSound;
        [SerializeField] private float soundRange = 10f;

        private float cooldownTimer;

        private void Update()
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= attackCooldown && PlayerIsVisible())
            {
                Attack();
            }
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision) { }

        private void Attack()
        {
            cooldownTimer = 0;

            if (PlayerReference.IsValid && Vector3.Distance(transform.position, PlayerReference.Transform.position) <= soundRange)
            {
                SoundManager.Instance?.PlaySound(arrowSound);
            }

            if (ObjectPoolManager.Instance != null)
            {
                var arrow = ObjectPoolManager.Instance.SpawnFromPool(arrowTag, firePoint.position, firePoint.rotation);
                arrow?.GetComponent<EnemyProjectile>()?.ActivateProjectile();
            }
        }

        private bool PlayerIsVisible()
        {
            var controller = PlayerReference.Controller;
            return controller != null && !controller.IsInvisible();
        }
    }
}
