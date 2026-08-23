using UnityEngine;
using Core.Constants;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    using Player = Gameplay.Characters.Player.Player;

    [RequireComponent(typeof(Animator), typeof(Health))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [SerializeField] protected EnemyConfigSO config;
        [SerializeField] protected Transform playerTransform;

        protected Animator anim;
        protected Collider2D col;
        protected Health health;
        protected PatrolMovement patrol;
        protected Player player;
        protected bool IsDead => !health.IsAlive;
        protected bool PlayerVisible => player == null || !player.IsInvisible;

        protected float Damage => config.damage;
        protected float Speed => config.speed;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
            health = GetComponent<Health>();
            patrol = GetComponentInParent<PatrolMovement>();
            if (config != null && config.animatorController != null)
                anim.runtimeAnimatorController = config.animatorController;
            if (playerTransform == null) playerTransform = GameConstants.FindPlayer();
            player = playerTransform?.GetComponent<Player>();
        }

        protected bool TryResolveTarget()
        {
            if (playerTransform == null) playerTransform = GameConstants.FindPlayer();
            if (player == null && playerTransform != null) player = playerTransform.GetComponent<Player>();
            return playerTransform != null;
        }

        protected void SetPatrol(bool enabled)
        {
            if (patrol != null) patrol.enabled = enabled;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) collision.DamagePlayer(Damage);
        }
    }
}
