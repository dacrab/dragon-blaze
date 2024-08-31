using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    #region Fields
    public float speed;
    [SerializeField] private float resetTime;
    private float lifetime;
    private bool hit;

    private Animator anim;
    private BoxCollider2D coll;
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

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ShouldProcessCollision(collision)) return;

        HandleCollision();
        base.OnTriggerEnter2D(collision);
    }
    #endregion

    #region Public Methods
    public void ActivateProjectile()
    {
        ResetProjectile();
    }
    #endregion

    #region Private Methods
    private void InitializeComponents()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    private void MoveProjectile()
    {
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);
    }

    private void UpdateLifetime()
    {
        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            Deactivate();
    }

    private bool ShouldProcessCollision(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return true;

        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        return playerMovement == null || playerMovement.IsVisible();
    }

    private void HandleCollision()
    {
        hit = true;
        coll.enabled = false;

        if (anim != null)
            anim.SetTrigger("explode");
        else
            Deactivate();
    }

    private void ResetProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    #endregion
}