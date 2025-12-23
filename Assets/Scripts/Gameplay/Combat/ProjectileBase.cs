using UnityEngine;
using Core.Constants;
using Core.Utilities;

namespace Gameplay.Combat
{
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
            col?.SetEnabled(false);
            if (anim != null) anim.SetTrigger("explode");
            else Deactivate();
        }

        public virtual void SetDirection(float dir)
        {
            ResetProjectile(dir);
            float localScaleX = Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir);
            transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        }

        protected virtual void ResetProjectile(float dir)
        {
            lifetime = 0;
            direction = dir;
            gameObject.SetActive(true);
            hit = false;
            col?.SetEnabled(true);
        }

        protected virtual void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
