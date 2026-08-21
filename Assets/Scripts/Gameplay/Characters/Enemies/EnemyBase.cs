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

        protected Animator anim;
        protected Collider2D col;
        protected Health health;
        protected bool IsDead => !health.IsAlive;

        protected float Damage => config.damage;
        protected float Speed => config.speed;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
            health = GetComponent<Health>();
            if (config != null && config.animatorController != null)
                anim.runtimeAnimatorController = config.animatorController;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) collision.DamagePlayer(Damage);
        }
    }
}
