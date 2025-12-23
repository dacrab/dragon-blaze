using UnityEngine;
using Core.Constants;
using Gameplay.Characters.Player;

namespace Gameplay.Characters.Enemies
{
    public class MeleeEnemy : EnemyBase
    {
        [Header("Attack Parameters")]
        [SerializeField] private float attackCooldown;
        [SerializeField] private float range;
        // Damage inherited from Base

        [Header("AI Parameters")]
        [SerializeField] private float chaseSpeed = 3.0f; // Differentiate from Patrol speed

        [Header("Player Detection")]
        [SerializeField] private LayerMask playerLayer;

        private float cooldownTimer = Mathf.Infinity;
        private Transform playerTransform;
        private EnemyPatrol enemyPatrol;
        private PlayerController playerController;

        protected override void Awake()
        {
            base.Awake();
            InitializeComponents();
        }

        private void Update()
        {
            if (isDead) return;
            if (!ValidateComponents()) return;

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

        private void InitializeComponents()
        {
            // In inherited classes, we might need to be careful not to override base unless intended.
            // But here base only gets RB/Anim/Col.
            
            enemyPatrol = GetComponentInParent<EnemyPatrol>();
            GameObject player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (player != null)
            {
                playerTransform = player.transform;
                playerController = player.GetComponent<PlayerController>();
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

            if (cooldownTimer >= attackCooldown)
            {
                Attack();
            }
        }

        private void Attack()
        {
            cooldownTimer = 0;
            anim.SetTrigger(GameConstants.Animation.MeleeAttack);
            // Damage handled via Animation Event calling DamagePlayer? 
            // Or instant? Original code called DamagePlayer() immediately.
            DamagePlayer();
        }

        private bool PlayerInSight()
        {
            return playerController != null && !playerController.IsInvisible();
        }

        private bool PlayerWithinPatrolBounds()
        {
            if (enemyPatrol == null || enemyPatrol.LeftEdge == null || enemyPatrol.RightEdge == null)
                return true; // Default to true if no patrol bounds to restrict chasing (e.g. boss)

            return playerTransform.position.x >= enemyPatrol.LeftEdge.position.x &&
                   playerTransform.position.x <= enemyPatrol.RightEdge.position.x;
        }

        private bool CanMoveForward()
        {
            // Original code used BoxCollider overlap check.
            // Reusing 'col' from base if it's a BoxCollider2D
            BoxCollider2D box = col as BoxCollider2D;
            if (box == null) return true;

            Vector2 direction = transform.right * transform.localScale.x;
            Vector2 checkPosition = (Vector2)transform.position + (direction * box.size.x);

            // Using GameConstants.Layers.Default? Or assuming LayerMask is int.
            // LayerMask.GetMask requires strings.
            Collider2D hit = Physics2D.OverlapBox(checkPosition, box.size, 0, LayerMask.GetMask("Default")); 
            return hit == null;
        }

        private void FollowPlayer()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) < range)
            {
                 // Stop if in range? Original didn't stop explicitly in FollowPlayer but MoveTowardsPlayer logic
                 // The original logic just moved towards X.
            }
            
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            float proposedXPosition = transform.position.x + direction.x * chaseSpeed * Time.deltaTime;

            // Check bounds again for movement target?
            // Reuse patrol bounds check logic if needed.
            if (PlayerWithinPatrolBounds()) // Actually we need to check if proposed position is within bounds
            {
                // We need strict bound check on self, not player
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
            // Check distance or collider overlap?
            // Original code just assumed if Attack() is called, it hits if in range? 
            // Actually original checked cooldown then Attack() then DamagePlayer().
            // But usually attack has a hitbox.
            // I'll implement a simple distance check to be fair.
            
            if (Vector3.Distance(transform.position, playerTransform.position) <= range + 1.0f) // + buffer
            {
                 Gameplay.Health.Health playerHealth = playerTransform.GetComponent<Gameplay.Health.Health>();
                 if (playerHealth) playerHealth.TakeDamage(damage);
            }
        }
    }
}
