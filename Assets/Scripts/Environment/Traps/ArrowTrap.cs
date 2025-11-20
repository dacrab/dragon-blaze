using UnityEngine;
using Core.Managers;
using Core.Constants;
using Gameplay.Characters.Player;
using Gameplay.Combat;
using Core.Optimization;

namespace Environment.Traps
{
    public class ArrowTrap : TrapBase
    {
        #region Serialized Fields
        [SerializeField] private float attackCooldown;
        [SerializeField] private Transform firePoint;
        [SerializeField] private string arrowTag = "Arrow"; // Tag for pooling

        [Header("SFX")]
        [SerializeField] private AudioClip arrowSound;
        [SerializeField] private float soundRange = 10f;
        #endregion

        #region Private Fields
        private float cooldownTimer;
        private Transform playerTransform;
        private PlayerController playerController;
        #endregion

        #region Unity Lifecycle Methods
        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (player != null)
            {
                playerTransform = player.transform;
                playerController = player.GetComponent<PlayerController>();
            }
        }

        private void Update()
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= attackCooldown && PlayerIsVisible())
            {
                Attack();
            }
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Do nothing. Arrow trap deals damage via arrows.
        }

        #endregion

        #region Private Methods
        private void Attack()
        {
            cooldownTimer = 0;

            if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= soundRange)
            {
                SoundManager.instance.PlaySound(arrowSound);
            }

            if (ObjectPoolManager.Instance != null)
            {
                GameObject arrow = ObjectPoolManager.Instance.SpawnFromPool(arrowTag, firePoint.position, firePoint.rotation);
                if (arrow != null)
                {
                     arrow.GetComponent<EnemyProjectile>().ActivateProjectile();
                }
            }
        }

        private bool PlayerIsVisible()
        {
            return playerController != null && !playerController.IsInvisible();
        }
        #endregion
    }
}
