using UnityEngine;
using Core.Constants;
using Core.Managers;

namespace Gameplay.Combat
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected float speed = 10f;
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected float maxLifetime = 5f;

        [Header("Effects")]
        [SerializeField] protected GameObject hitEffectPrefab;
        [SerializeField] protected AudioClip hitSound;

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

        protected virtual void OnEnable() => ResetState();

        protected virtual void ResetState()
        {
            lifetime = 0f;
            hit = false;
            if (col != null) col.enabled = true;
        }

        protected virtual void Update()
        {
            if (hit) return;
            transform.Translate(speed * Time.deltaTime * direction, 0, 0);
            lifetime += Time.deltaTime;
            if (lifetime > maxLifetime) Deactivate();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (hit || collision.transform.root == transform.root) return;
            hit = true;
            if (col != null) col.enabled = false;
            
            if (hitEffectPrefab != null)
            {
                Vector3 hitPoint = collision != null ? collision.ClosestPoint(transform.position) : transform.position;
                Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
            }
            SoundManager.Instance?.PlaySound(hitSound);
            
            if (anim != null) anim.SetTrigger(Core.Constants.GameConstants.Animation.Explode);
            else Deactivate();
        }

        public virtual void SetDirection(float dir)
        {
            direction = dir;
            lifetime = 0;
            hit = false;
            gameObject.SetActive(true);
            if (col != null) col.enabled = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir), transform.localScale.y, transform.localScale.z);
        }

        public virtual void OnExplosionComplete() => Deactivate();
        protected virtual void Deactivate() => gameObject.SetActive(false);
    }
}
