using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Physics;
using Core.Pooling;
using Core.Services;

namespace Gameplay.Combat
{
    using Player = Gameplay.Characters.Player.Player;

    [RequireComponent(typeof(Animator), typeof(Collider2D))]
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected float speed = 10f, damage = 1f, maxLifetime = 5f;
        [SerializeField] protected GameObject hitEffectPrefab;
        [SerializeField] protected AudioClip hitSound;
        [SerializeField] protected string targetTag = GameConstants.Tags.Enemy;
        [SerializeField] protected bool checkInvisibility;

        protected float lifetime, direction;
        protected bool hit;
        protected Animator? anim;
        protected Collider2D? col;
        protected Rigidbody2D? body;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
            body = KinematicBody.Prepare(this);
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
            lifetime += Time.deltaTime;
            if (lifetime > maxLifetime) Deactivate();
        }

        protected virtual void FixedUpdate()
        {
            if (hit) return;
            var step = new Vector3(speed * direction * Time.fixedDeltaTime, 0f, 0f);
            KinematicBody.MoveTo(body, transform, transform.position + step);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (hit || collision.transform.root == transform.root) return;
            if (collision.CompareTag(targetTag))
                collision.DamagePlayer(damage, checkInvisibility);

            hit = true;
            if (col != null) col.enabled = false;
            VfxPool.Spawn(hitEffectPrefab, collision.ClosestPoint(transform.position), Quaternion.identity);
            ServiceLocator.Get<IAudioManager>()?.PlaySound(hitSound);
            if (anim != null) anim.SetTrigger(GameConstants.Anim.Explode);
            else Deactivate();
        }

        public virtual void SetDirection(float dir)
        {
            direction = dir;
            gameObject.SetActive(true);
            transform.localScale = new(Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir), transform.localScale.y, transform.localScale.z);
        }

        public void ActivateProjectile() => SetDirection(transform.lossyScale.x > 0 ? 1 : -1);
        public void OnExplosionComplete() => Deactivate();

        /// <summary>Cycles a shared projectile pool: takes the next live entry and moves it to <paramref name="position"/>.
        /// Callers arm it afterwards via SetDirection.</summary>
        public static ProjectileBase? Fire(GameObject[] projectiles, ref int index, Vector3 position)
        {
            if (projectiles is not { Length: > 0 }) return null;
            // Advance only past entries that actually yielded a projectile, so an empty/placeholder slot
            // cannot burn a turn or throw on a missing transform.
            for (int i = 0; i < projectiles.Length; i++)
            {
                int slot = (index + i) % projectiles.Length;
                if (projectiles[slot] is not { } candidate) continue;
                if (!candidate.TryGetComponent<ProjectileBase>(out var proj)) continue;
                index = (slot + 1) % projectiles.Length;
                candidate.transform.position = position;
                return proj;
            }
            return null;
        }

        protected void Deactivate() => gameObject.SetActive(false);
    }
}
