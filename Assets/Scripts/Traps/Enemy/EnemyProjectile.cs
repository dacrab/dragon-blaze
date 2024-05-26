using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    public float speed; // Now publicly accessible
    [SerializeField] private float resetTime;
    private float lifetime;
    private Animator anim;
    private BoxCollider2D coll;

    private bool hit;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }

    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            gameObject.SetActive(false);
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        coll.enabled = false;

        if (anim != null)
            anim.SetTrigger("explode");
        else
            gameObject.SetActive(false);

        base.OnTriggerEnter2D(collision);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
