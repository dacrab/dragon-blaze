using UnityEngine;
using Core.Constants;
using Core.Utilities;
using Gameplay.Characters.Player;

namespace Gameplay.Characters.Enemies
{
    public class MeleeEnemy : EnemyBase
    {
        [Header("Attack Parameters")]
        [SerializeField] private float attackCooldown;
        [SerializeField] private float range;

        [Header("AI Parameters")]
        [SerializeField] private float chaseSpeed = 3.0f;

        [Header("Player Detection")]
        [SerializeField] private LayerMask playerLayer;

        private float cooldownTimer = Mathf.Infinity;
        private EnemyPatrol enemyPatrol;

        protected override void Awake()
        {
            base.Awake();
            enemyPatrol = GetComponentInParent<EnemyPatrol>();
        }

        private void Update()
        {
            if (isDead || !PlayerReference.IsValid) return;

            cooldownTimer += Time.deltaTime;

            if (PlayerInSight() && PlayerWithinPatrolBounds())
            {
                HandlePlayerDetected();
            }
            else
            {
                if (enemyPatrol != null) enemyPatrol.enabled = true;
            }
        }

        private void HandlePlayerDetected()
        {
            if (enemyPatrol != null) enemyPatrol.enabled = false;
            
            if (CanMoveForward())
            {
                FollowPlayer();
            }

            if (cooldownTimer >= attackCooldown)
            {
                Attack();
            }
        }

        private void Attack()
        {
            cooldownTimer = 0;
            anim.SetTrigger(GameConstants.Animation.MeleeAttack);
            DamagePlayer();
        }

        private bool PlayerInSight()
        {
            var controller = PlayerReference.Controller;
            return controller != null && !controller.IsInvisible();
        }

        private bool PlayerWithinPatrolBounds()
        {
            if (enemyPatrol == null || enemyPatrol.LeftEdge == null || enemyPatrol.RightEdge == null)
                return true;

            var playerPos = PlayerReference.Transform.position;
            return playerPos.x >= enemyPatrol.LeftEdge.position.x &&
                   playerPos.x <= enemyPatrol.RightEdge.position.x;
        }

        private bool CanMoveForward()
        {
            var box = col as BoxCollider2D;
            if (box == null) return true;

            Vector2 direction = transform.right * transform.localScale.x;
            Vector2 checkPosition = (Vector2)transform.position + (direction * box.size.x);
            Collider2D hit = Physics2D.OverlapBox(checkPosition, box.size, 0, LayerMask.GetMask("Default")); 
            return hit == null;
        }

        private void FollowPlayer()
        {
            var playerPos = PlayerReference.Transform.position;
            Vector3 direction = (playerPos - transform.position).normalized;
            float proposedXPosition = transform.position.x + direction.x * chaseSpeed * Time.deltaTime;

            if (enemyPatrol == null || (proposedXPosition >= enemyPatrol.LeftEdge.position.x && proposedXPosition <= enemyPatrol.RightEdge.position.x))
            {
                MoveTowardsPlayer(proposedXPosition, direction);
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
            var playerTransform = PlayerReference.Transform;
            if (Vector3.Distance(transform.position, playerTransform.position) <= range + 1.0f)
            {
                var playerHealth = PlayerReference.GetComponent<Gameplay.Health.Health>();
                if (playerHealth != null)
                {
                    float dmg = stats != null ? stats.damage : 10f;
                    playerHealth.TakeDamage(dmg);
                }
            }
        }
    }
}
