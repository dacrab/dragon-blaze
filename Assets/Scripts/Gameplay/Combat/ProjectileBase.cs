using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Pooling;

namespace Gameplay.Combat
{
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected float speed = 10f, damage = 1f, maxLifetime = 5f;
        [SerializeField] protected GameObject hitEffectPrefab;
        [SerializeField] protected AudioClip hitSound;
        [SerializeField] protected string targetTag = GameConstants.Tags.Enemy;
        [SerializeField] protected bool checkInvisibility;
        [SerializeField] protected string poolKey;

        protected float lifetime, direction;
        protected bool hit;
        protected Animator anim;
        protected Collider2D col;

        public bool IsPooled => !string.IsNullOrEmpty(poolKey);

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
        }

        protected virtual void OnEnable()
        {
            lifetime = 0f;
            hit = false;
            if (col != null) col.enabled = true;
        }

        protected virtual void Update()
        {
            if (hit) return;
            transform.Translate(speed * direction * Time.deltaTime, 0, 0);
            lifetime += Time.deltaTime;
            if (lifetime > maxLifetime) Deactivate();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (hit || collision.transform.root == transform.root) return;

            if (collision.CompareTag(targetTag))
            {
                if (checkInvisibility && collision.TryGetComponent<Characters.Player.Player>(out var player) && player.IsInvisible)
                    return;
                if (collision.TryGetComponent<Health>(out var target))
                    target.TakeDamage(damage);
            }

            hit = true;
            if (col != null) col.enabled = false;
            if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, collision.ClosestPoint(transform.position), Quaternion.identity);
            GameManager.Instance?.PlaySound(hitSound);
            if (anim != null) anim.SetTrigger(GameConstants.Anim.Explode);
            else Deactivate();
        }

        public virtual void SetDirection(float dir)
        {
            direction = dir;
            lifetime = 0;
            hit = false;
            gameObject.SetActive(true);
            if (col != null) col.enabled = true;
            transform.localScale = new(Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir), transform.localScale.y, transform.localScale.z);
        }

        public void ActivateProjectile() => SetDirection(transform.lossyScale.x > 0 ? 1 : -1);
        public void OnExplosionComplete() => Deactivate();

        protected void Deactivate()
        {
            if (IsPooled) PoolRegistry.Release(poolKey, gameObject);
            else gameObject.SetActive(false);
        }
    }
}
