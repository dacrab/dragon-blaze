using UnityEngine;
using Core.Constants;

namespace Gameplay.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected float damage = 1f; // Default damage
        [SerializeField] protected float maxLifetime = 5f;

        protected float lifetime;
        protected float direction;
        protected bool hit;
        protected Animator anim;
        protected Collider2D col;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
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
            if (lifetime > maxLifetime) Deactivate();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (hit) return;
            
            hit = true;
            if (col) col.enabled = false;

            if (anim != null)
                anim.SetTrigger("explode"); 
            else
                Deactivate(); 
        }

        public virtual void SetDirection(float _direction)
        {
            ResetProjectile(_direction);
            
            // Flip visual
            float localScaleX = Mathf.Abs(transform.localScale.x) * Mathf.Sign(_direction);
            transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        }

        protected virtual void ResetProjectile(float _direction)
        {
            lifetime = 0;
            direction = _direction;
            gameObject.SetActive(true);
            hit = false;
            if (col) col.enabled = true;
        }

        protected virtual void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
