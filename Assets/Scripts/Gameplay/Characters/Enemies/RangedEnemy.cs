using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.Utilities;
using Gameplay.Characters.Player;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    /// <summary>
    /// Ranged enemy that shoots projectiles at the player.
    /// </summary>
    public class RangedEnemy : EnemyBase
    {
        #region Serialized Fields
        [Header("Attack Parameters")]
        [SerializeField] private float attackCooldownDuration = CombatConstants.DefaultAttackCooldown;
        [SerializeField] private float range = 10f;
        [SerializeField] private float colliderDistance = 0.5f;

        [Header("Ranged Attack")]
        [SerializeField] private Transform firepoint;
        [SerializeField] private GameObject[] fireballs;
        [SerializeField] private bool usePooling = false;
        [SerializeField] private string projectilePoolTag = "EnemyFireball";

        [Header("Player Detection")]
        [SerializeField] private LayerMask playerLayer;

        [Header("Audio")]
        [SerializeField] private AudioClip fireballSound;
        #endregion

        #region Private Fields
        private CooldownTimer attackCooldown;
        [AutoWire(AutoWireAttribute.WireType.Parent, required: false)]
        [SerializeField] private EnemyPatrol enemyPatrol;
        private PlayerController playerController;
        private Transform playerTransform;
        #endregion

        #region Unity Lifecycle Methods
        protected override void Awake()
        {
            base.Awake();
            Core.Utilities.AutoWireHelper.WireAllFields(this);
            attackCooldown = new CooldownTimer(attackCooldownDuration);
            InitializeComponents();
        }

        private void Update()
        {
            if (isDead || !GameStateHelpers.IsPlaying) return;

            attackCooldown.Update();
            
            if (PlayerInSight())
            {
                if (attackCooldown.IsReady)
                {
                    attackCooldown.Reset();
                    anim?.SetTrigger("rangedAttack");
                }
                
                if (enemyPatrol != null) enemyPatrol.enabled = false;
            }
            else
            {
                if (enemyPatrol != null) enemyPatrol.enabled = true;
            }
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            // enemyPatrol is auto-wired via [AutoWire]
            if (PlayerReference.IsValid)
            {
                playerTransform = PlayerReference.Transform;
                playerController = PlayerReference.Controller;
            }
        }
        #endregion

        #region Attack Methods
        /// <summary>
        /// Called by Animation Event to fire projectile.
        /// </summary>
        private void RangedAttack()
        {
            SoundManager.Instance?.PlaySound(fireballSound);
            attackCooldown.Reset();
            
            SpawnProjectile();
        }

        private void SpawnProjectile()
        {
            if (firepoint == null) return;

            if (usePooling)
            {
                var projectile = Core.Optimization.ObjectPoolManager.Instance?.Get(
                    projectilePoolTag,
                    firepoint.position,
                    Quaternion.identity
                );
                
                if (projectile != null)
                {
                    var enemyProjectile = projectile.GetComponent<EnemyProjectile>();
                    enemyProjectile?.ActivateProjectile();
                }
            }
            else
            {
                GameObject fireball = GetAvailableFireball();
                if (fireball != null)
                {
                    fireball.transform.position = firepoint.position;
                    fireball.GetComponent<EnemyProjectile>()?.ActivateProjectile();
                }
            }
        }

        private GameObject GetAvailableFireball()
        {
            if (fireballs == null) return null;
            
            for (int i = 0; i < fireballs.Length; i++)
            {
                if (!fireballs[i].activeInHierarchy)
                    return fireballs[i];
            }
            return null;
        }
        #endregion

        #region Player Detection Methods
        private bool PlayerInSight()
        {
            if (playerController == null || playerController.IsInvisible()) return false;

            BoxCollider2D box = col as BoxCollider2D;
            if (box == null) return false;

            RaycastHit2D hit = Physics2D.BoxCast(
                box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(box.bounds.size.x * range, box.bounds.size.y, box.bounds.size.z),
                0, Vector2.left, 0, playerLayer);

            return hit.collider != null && hit.collider.CompareTag(GameConstants.Tags.Player);
        }
        #endregion

        #region Gizmo Methods
        private void OnDrawGizmos()
        {
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if (box == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                box.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(box.bounds.size.x * range, box.bounds.size.y, box.bounds.size.z)
            );
        }
        #endregion
    }
}
