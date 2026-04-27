using UnityEngine;
using Core.Constants;
using Gameplay.Combat;

namespace Gameplay.Characters.Enemies
{
    [RequireComponent(typeof(Animator), typeof(Health))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected float speed = 3f;
        
        [Header("Death")]
        [SerializeField] protected float deathDelay = 2f;

        protected Animator anim;
        protected Collider2D col;
        protected Health health;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
            health = GetComponent<Health>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            if (collision.GetComponent<Player.Player>() is { IsInvisible: true }) return;
            collision.GetComponent<Health>()?.TakeDamage(damage);
        }
    }
}