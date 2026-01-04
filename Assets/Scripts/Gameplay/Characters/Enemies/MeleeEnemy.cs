using UnityEngine;
using Core.State;

namespace Gameplay.Characters.Enemies
{
    public class MeleeEnemy : EnemyBase
    {
        [Header("Attack")]
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float range = 1f;

        [Header("AI")]
        [SerializeField] private float chaseSpeed = 3.0f;

        private float cooldownTimer;
        private Transform playerTransform;
        private EnemyPatrol enemyPatrol;
        private Gameplay.Characters.Player.PlayerController playerController;

        protected override void Awake()
        {
            base.Awake();
            enemyPatrol = GetComponentInParent<EnemyPatrol>();
            var player = GameObject.FindGameObjectWithTag(Core.Constants.GameConstants.Tags.Player);
            if (player != null)
            {
                playerTransform = player.transform;
                playerController = player.GetComponent<Gameplay.Characters.Player.PlayerController>();
            }
        }

        private void Update()
        {
            if (isDead || !GameStateManager.Instance.IsPlaying) return;
            if (playerController == null || playerTransform == null) return;

            cooldownTimer += Time.deltaTime;

            if (PlayerInSight() && PlayerWithinPatrolBounds())
            {
                if (enemyPatrol != null) enemyPatrol.enabled = false;
                if (CanMoveForward()) FollowPlayer();
                if (cooldownTimer >= attackCooldown) Attack();
            }
            else if (enemyPatrol != null) enemyPatrol.enabled = true;
        }

        private void Attack()
        {
            cooldownTimer = 0f;
            anim.SetTrigger(Core.Constants.GameConstants.Animation.MeleeAttack);
            if (Vector3.Distance(transform.position, playerTransform.position) <= range + 1f)
            {
                var health = playerTransform.GetComponent<Gameplay.Health.Health>();
                health?.TakeDamage(damage);
            }
        }

        private bool PlayerInSight() => playerController != null && !playerController.IsInvisible();

        private bool PlayerWithinPatrolBounds()
        {
            if (enemyPatrol == null || enemyPatrol.LeftEdge == null || enemyPatrol.RightEdge == null) return true;
            return playerTransform.position.x >= enemyPatrol.LeftEdge.position.x &&
                   playerTransform.position.x <= enemyPatrol.RightEdge.position.x;
        }

        private bool CanMoveForward()
        {
            var box = col as BoxCollider2D;
            if (box == null) return true;
            Vector2 direction = transform.right * transform.localScale.x;
            Vector2 checkPos = (Vector2)transform.position + (direction * box.size.x);
            return Physics2D.OverlapBox(checkPos, box.size, 0, LayerMask.GetMask("Default")) == null;
        }

        private void FollowPlayer()
        {
            Vector3 dir = (playerTransform.position - transform.position).normalized;
            float proposedX = transform.position.x + dir.x * chaseSpeed * Time.deltaTime;

            if (enemyPatrol == null || (proposedX >= enemyPatrol.LeftEdge.position.x && proposedX <= enemyPatrol.RightEdge.position.x))
            {
                transform.position = new Vector3(proposedX, transform.position.y, transform.position.z);
                anim.SetBool(Core.Constants.GameConstants.Animation.Moving, true);
                transform.localScale = new Vector3(dir.x > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
