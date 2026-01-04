using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;
using Gameplay.Combat;

namespace Environment.Traps
{
    public class ArrowTrap : TrapBase
    {
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject[] arrows;
        [SerializeField] private AudioClip arrowSound;
        [SerializeField] private float soundRange = 10f;

        private float cooldownTimer;
        private EnemyProjectile[] cachedProjectiles;
        private Transform playerTransform;
        private Gameplay.Characters.Player.PlayerController playerController;

        private void Awake()
        {
            CacheProjectiles();
            var player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (player != null)
            {
                playerTransform = player.transform;
                playerController = player.GetComponent<Gameplay.Characters.Player.PlayerController>();
            }
        }

        private void CacheProjectiles()
        {
            if (arrows == null || arrows.Length == 0) return;
            cachedProjectiles = new EnemyProjectile[arrows.Length];
            for (int i = 0; i < arrows.Length; i++)
                if (arrows[i] != null)
                    cachedProjectiles[i] = arrows[i].GetComponent<EnemyProjectile>();
        }

        private void Update()
        {
            if (!GameStateManager.Instance.IsPlaying) return;
            
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown && PlayerIsVisible()) Attack();
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision) { }

        private void Attack()
        {
            cooldownTimer = 0;
            
            if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= soundRange)
                SoundManager.Instance?.PlaySound(arrowSound);

            int idx = System.Array.FindIndex(arrows, a => !a.activeInHierarchy);
            if (idx < 0) idx = 0;
            
            arrows[idx].transform.position = firePoint.position;
            cachedProjectiles[idx]?.ActivateProjectile();
        }

        private bool PlayerIsVisible() => playerController != null && !playerController.IsInvisible();
    }
}
