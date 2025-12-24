using UnityEngine;
using Core.Combat;
using Core.Interfaces;
using Core.Constants;
using Core.Utilities;
using Gameplay.Characters.Player;

namespace Gameplay.Characters.Enemies
{
    public class MeleeEnemy : EnemyBase
    {
        [Header("Attack Parameters")]
        [SerializeField] private float attackCooldownDuration = CombatConstants.DefaultAttackCooldown;
        [SerializeField] private float range = 1f;
        [SerializeField] private float attackRangeBuffer = CombatConstants.AttackRangeBuffer;

        [Header("AI Parameters")]
        [SerializeField] private float chaseSpeed = 3.0f;

        [Header("Player Detection")]
        [SerializeField] private LayerMask playerLayer;

        private CooldownTimer attackCooldown;
        private Transform playerTransform;
        [AutoWire(AutoWireAttribute.WireType.Parent, required: false)]
        [SerializeField] private EnemyPatrol enemyPatrol;
        private PlayerController playerController;

        protected override void Awake()
        {
            base.Awake();
            AutoWireHelper.WireAllFields(this);
            attackCooldown = new CooldownTimer(attackCooldownDuration);
            InitializeComponents();
        }

        private void Update()
        {
            if (isDead || !GameStateHelpers.IsPlaying) return;
            if (!ValidateComponents()) return;

            attackCooldown.Update();

            if (PlayerInSight() && PlayerWithinPatrolBounds())
            {
                HandlePlayerDetected();
            }
            else
            {
                if (enemyPatrol != null) enemyPatrol.enabled = true;
            }
        }

        private void InitializeComponents()
        {
            // enemyPatrol is auto-wired via [AutoWire]
            if (PlayerReference.IsValid)
            {
                playerTransform = PlayerReference.Transform;
                playerController = PlayerReference.Controller;
            }
        }

        private bool ValidateComponents()
        {
            return playerController != null && playerTransform != null;
        }

        private void HandlePlayerDetected()
        {
            if (enemyPatrol != null) enemyPatrol.enabled = false;
            
            if (CanMoveForward())
            {
                FollowPlayer();
            }

            if (attackCooldown.IsReady)
            {
                Attack();
            }
        }

        private void Attack()
        {
            attackCooldown.Reset();
            anim.SetTrigger(GameConstants.Animation.MeleeAttack);
            DamagePlayer();
        }

        private bool PlayerInSight()
        {
            return playerController != null && !playerController.IsInvisible();
        }

        private bool PlayerWithinPatrolBounds()
        {
            if (enemyPatrol == null || enemyPatrol.LeftEdge == null || enemyPatrol.RightEdge == null)
                return true;

            return playerTransform.position.x >= enemyPatrol.LeftEdge.position.x &&
                   playerTransform.position.x <= enemyPatrol.RightEdge.position.x;
        }

        private bool CanMoveForward()
        {
            BoxCollider2D box = col as BoxCollider2D;
            if (box == null) return true;

            Vector2 direction = transform.right * transform.localScale.x;
            Vector2 checkPosition = (Vector2)transform.position + (direction * box.size.x);

            Collider2D hit = Physics2D.OverlapBox(checkPosition, box.size, 0, LayerConstants.GetMask(LayerConstants.DefaultLayer)); 
            return hit == null;
        }

        private void FollowPlayer()
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            float proposedXPosition = transform.position.x + direction.x * chaseSpeed * Time.deltaTime;

            if (PlayerWithinPatrolBounds())
            {
                if (enemyPatrol == null || (proposedXPosition >= enemyPatrol.LeftEdge.position.x && proposedXPosition <= enemyPatrol.RightEdge.position.x))
                {
                    MoveTowardsPlayer(proposedXPosition, direction);
                }
            }
        }

        private void MoveTowardsPlayer(float proposedXPosition, Vector3 direction)
        {
            transform.position = new Vector3(proposedXPosition, transform.position.y, transform.position.z);
            anim.SetBool(GameConstants.Animation.Moving, true);

            if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        private void DamagePlayer()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= range + attackRangeBuffer)
            {
                var damageInfo = DamageInfo.Physical(damage, gameObject);
                
                if (playerTransform.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(damageInfo);
                else if (playerTransform.TryGetHealth(out var health))
                    health.TakeDamage(damage);
            }
        }
    }
}
