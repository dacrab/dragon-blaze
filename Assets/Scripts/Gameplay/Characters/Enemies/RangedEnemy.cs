using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    public class RangedEnemy : EnemyBase
    {
        [Header("Attack")]
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float range = 10f;
        [SerializeField] private float colliderDistance = 0.5f;

        [Header("Ranged Attack")]
        [SerializeField] private Transform firepoint;
        [SerializeField] private GameObject[] fireballs;

        [Header("Detection")]
        [SerializeField] private LayerMask playerLayer;

        [Header("Audio")]
        [SerializeField] private AudioClip fireballSound;

        private float cooldownTimer;
        private EnemyPatrol enemyPatrol;
        private Gameplay.Characters.Player.PlayerController playerController;

        protected override void Awake()
        {
            base.Awake();
            enemyPatrol = GetComponentInParent<EnemyPatrol>();
            var player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (player != null) playerController = player.GetComponent<Gameplay.Characters.Player.PlayerController>();
        }

        private void Update()
        {
            if (isDead || !GameStateManager.IsCurrentlyPlaying) return;

            cooldownTimer += Time.deltaTime;
            
            if (PlayerInSight())
            {
                if (cooldownTimer >= attackCooldown)
                {
                    cooldownTimer = 0f;
                    anim?.SetTrigger(GameConstants.Animation.RangedAttack);
                }
                if (enemyPatrol != null) enemyPatrol.enabled = false;
            }
            else if (enemyPatrol != null) enemyPatrol.enabled = true;
        }

        private void RangedAttack()
        {
            SoundManager.Instance?.PlaySound(fireballSound);
            if (firepoint == null || fireballs == null) return;
            
            var fireball = System.Array.Find(fireballs, f => !f.activeInHierarchy);
            if (fireball != null)
            {
                fireball.transform.position = firepoint.position;
                fireball.GetComponent<EnemyProjectile>()?.ActivateProjectile();
            }
        }

        private bool PlayerInSight()
        {
            if (playerController == null || playerController.IsInvisible()) return false;
            var box = col as BoxCollider2D;
            if (box == null) return false;

            RaycastHit2D hit = Physics2D.BoxCast(
                box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(box.bounds.size.x * range, box.bounds.size.y, box.bounds.size.z),
                0, Vector2.left, 0, playerLayer);

            return hit.collider != null && hit.collider.CompareTag(GameConstants.Tags.Player);
        }

        private void OnDrawGizmos()
        {
            var box = GetComponent<BoxCollider2D>();
            if (box == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(box.bounds.size.x * range, box.bounds.size.y, box.bounds.size.z));
        }
    }
}
