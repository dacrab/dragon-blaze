using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.Utilities;
using Gameplay.Combat;

namespace Environment.Traps
{
    public class ArrowTrap : TrapBase
    {
        [SerializeField] private float attackCooldown = CombatConstants.DefaultAttackCooldown;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject[] arrows;
        [SerializeField] private AudioClip arrowSound;
        [SerializeField] private float soundRange = 10f;

        private float cooldownTimer;
        private EnemyProjectile[] cachedProjectiles;

        private void Awake()
        {
            CacheProjectiles();
        }

        private void CacheProjectiles()
        {
            if (arrows == null || arrows.Length == 0) return;
            cachedProjectiles = new EnemyProjectile[arrows.Length];
            for (int i = 0; i < arrows.Length; i++)
            {
                if (arrows[i] != null)
                    cachedProjectiles[i] = arrows[i].GetComponent<EnemyProjectile>();
            }
        }

        private void Update()
        {
            if (!GameStateHelpers.IsPlaying) return;
            
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown && PlayerIsVisible()) Attack();
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision) { }

        private void Attack()
        {
            cooldownTimer = 0;
            
            if (PlayerReference.IsValid 
                && Vector3.Distance(transform.position, PlayerReference.Transform.position) <= soundRange)
                SoundManager.Instance?.PlaySound(arrowSound);

            int arrowIndex = System.Array.FindIndex(arrows, a => !a.activeInHierarchy);
            if (arrowIndex < 0) arrowIndex = 0;
            
            arrows[arrowIndex].transform.position = firePoint.position;
            cachedProjectiles[arrowIndex]?.ActivateProjectile();
        }

        private bool PlayerIsVisible() => PlayerReference.IsValid 
            && PlayerReference.Controller != null 
            && !PlayerReference.Controller.IsInvisible();
    }
}
