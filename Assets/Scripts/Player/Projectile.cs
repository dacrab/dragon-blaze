using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private float speed;
    #endregion

    #region Private Fields
    private float direction;
    private bool hit;
    private float lifetime;

    private Animator anim;
    private BoxCollider2D boxCollider;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        InitializeComponents();
    }

    private void Update()
    {
        if (hit) return;

        MoveProjectile();
        UpdateLifetime();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision);
    }
    #endregion

    #region Public Methods
    public void SetDirection(float _direction)
    {
        ResetProjectile(_direction);
        FlipProjectile(_direction);
    }
    #endregion

    #region Private Methods
    private void InitializeComponents()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void MoveProjectile()
    {
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
    }

    private void UpdateLifetime()
    {
        lifetime += Time.deltaTime;
        if (lifetime > 5) Deactivate();
    }

    private void HandleCollision(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");

        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Health>()?.TakeDamage(1);
        }
    }

    private void ResetProjectile(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;
    }

    private void FlipProjectile(float _direction)
    {
        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        }
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
