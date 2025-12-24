using UnityEngine;
using Core.Constants;
using Core.Interfaces;
using Core.Managers;
using Core.Utilities;

namespace Gameplay.Combat
{
    /// <summary>
    /// Base class for all projectiles. Implements IPoolable for object pooling support.
    /// </summary>
    public abstract class ProjectileBase : MonoBehaviour, IPoolable
    {
        [Header("Projectile Settings")]
        [SerializeField] protected float speed = 10f;
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected float maxLifetime = 5f;
        [SerializeField] protected bool usePooling = true;
        [SerializeField] protected string poolTag = "Projectile";

        [Header("Effects")]
        [SerializeField] protected GameObject hitEffectPrefab;
        [SerializeField] protected AudioClip hitSound;

        protected float lifetime;
        protected float direction;
        protected bool hit;
        [AutoWire(AutoWireAttribute.WireType.Self, required: false)]
        [SerializeField] protected Animator anim;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] protected Collider2D col;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] protected Rigidbody2D rb;
        [AutoWire(AutoWireAttribute.WireType.Self, required: false)]
        [SerializeField] protected TrailRenderer trail;

        protected virtual void Awake()
        {
            Core.Utilities.AutoWireHelper.WireAllFields(this);
        }

        #region IPoolable Implementation
        /// <summary>
        /// Called when retrieved from pool.
        /// </summary>
        public virtual void OnSpawn()
        {
            ResetState();
        }

        /// <summary>
        /// Called when returned to pool.
        /// </summary>
        public virtual void OnDespawn()
        {
            // Clear trail if present
            if (trail != null)
            {
                trail.Clear();
            }
        }
        #endregion

        protected virtual void OnEnable()
        {
            ResetState();
        }

        protected virtual void ResetState()
        {
            lifetime = 0f;
            hit = false;
            if (col != null) col.enabled = true;
        }

        protected virtual void Update()
        {
            if (hit) return;

            Move();
            UpdateLifetime();
        }

        protected virtual void Move()
        {
            float movementSpeed = speed * Time.deltaTime * direction;
            transform.Translate(movementSpeed, 0, 0);
        }

        protected virtual void UpdateLifetime()
        {
            lifetime += Time.deltaTime;
            if (lifetime > maxLifetime)
            {
                Deactivate();
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (hit) return;
            
            // Don't hit the shooter
            if (collision.transform.root == transform.root) return;
            
            hit = true;
            col?.SetEnabled(false);
            
            SpawnHitEffect(collision);
            PlayHitSound();
            
            if (anim != null)
            {
                anim.SetTrigger("explode");
            }
            else
            {
                Deactivate();
            }
        }

        public virtual void SetDirection(float dir)
        {
            direction = dir;
            lifetime = 0;
            hit = false;
            
            gameObject.SetActive(true);
            col?.SetEnabled(true);
            
            // Flip sprite based on direction
            float localScaleX = Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir);
            transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        }

        /// <summary>
        /// Sets direction with initial position.
        /// </summary>
        public virtual void Launch(Vector3 position, float dir)
        {
            transform.position = position;
            SetDirection(dir);
        }

        protected virtual void SpawnHitEffect(Collider2D target)
        {
            if (hitEffectPrefab != null)
            {
                Vector3 hitPoint = target != null 
                    ? target.ClosestPoint(transform.position) 
                    : transform.position;
                Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
            }
        }

        protected virtual void PlayHitSound()
        {
            if (hitSound != null)
            {
                SoundManager.Instance?.PlaySound(hitSound);
            }
        }

        /// <summary>
        /// Called by animation event when explosion animation completes.
        /// </summary>
        public virtual void OnExplosionComplete()
        {
            Deactivate();
        }

        protected virtual void Deactivate()
        {
            if (usePooling)
            {
                // Return to pool
                gameObject.ReturnToPool(poolTag);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

    }
}
