using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.Utilities;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    public class RangedEnemy : EnemyBase
    {
        [Header("Attack Parameters")]
        [SerializeField] private float attackCooldown;
        [SerializeField] private float range;

        [Header("Ranged Attack")]
        [SerializeField] private Transform firepoint;
        [SerializeField] private GameObject[] fireballs;

        [Header("Collider Parameters")]
        [SerializeField] private float colliderDistance;

        [Header("Player Detection")]
        [SerializeField] private LayerMask playerLayer;

        [Header("Audio")]
        [SerializeField] private AudioClip fireballSound;

        private float cooldownTimer = Mathf.Infinity;
        private EnemyPatrol enemyPatrol;

        protected override void Awake()
        {
            base.Awake();
            enemyPatrol = GetComponentInParent<EnemyPatrol>();
        }

        private void Update()
        {
            if (isDead) return;

            cooldownTimer += Time.deltaTime;
            
            if (PlayerInSight())
            {
                if (cooldownTimer >= attackCooldown)
                {
                    cooldownTimer = 0;
                    anim.SetTrigger("rangedAttack");
                }
                
                if (enemyPatrol != null) enemyPatrol.enabled = false;
            }
            else
            {
                if (enemyPatrol != null) enemyPatrol.enabled = true;
            }
        }

        private void RangedAttack()
        {
            SoundManager.Instance?.PlaySound(fireballSound);
            cooldownTimer = 0;
            
            var fireball = GetFireball();
            if (fireball != null)
            {
                fireball.transform.position = firepoint.position;
                fireball.GetComponent<EnemyProjectile>()?.ActivateProjectile();
            }
        }

        private GameObject GetFireball()
        {
            foreach (var fb in fireballs)
            {
                if (!fb.activeInHierarchy) return fb;
            }
            return null;
        }

        private bool PlayerInSight()
        {
            var controller = PlayerReference.Controller;
            if (controller == null || controller.IsInvisible()) return false;

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
            Gizmos.DrawWireCube(box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(box.bounds.size.x * range, box.bounds.size.y, box.bounds.size.z));
        }
    }
}
