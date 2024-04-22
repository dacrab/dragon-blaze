using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed; // Speed of the projectile
    private float direction; // Direction of the projectile (-1 for left, 1 for right)
    private bool hit; // Flag to track if the projectile has hit something
    private float lifetime; // Time the projectile has been active

    private Animator anim; // Reference to the Animator component
    private BoxCollider2D boxCollider; // Reference to the BoxCollider2D component

    private void Awake()
    {
        anim = GetComponent<Animator>(); // Get the Animator component
        boxCollider = GetComponent<BoxCollider2D>(); // Get the BoxCollider2D component
    }

    private void Update()
    {
        if (hit) return; // If the projectile has hit something, exit the update loop

        float movementSpeed = speed * Time.deltaTime * direction; // Calculate the movement speed
        transform.Translate(movementSpeed, 0, 0); // Move the projectile horizontally

        lifetime += Time.deltaTime; // Increase the lifetime of the projectile
        if (lifetime > 5) gameObject.SetActive(false); // Deactivate the projectile after a certain lifetime
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true; // Set hit flag to true
        boxCollider.enabled = false; // Disable the collider to prevent further collisions
        anim.SetTrigger("explode"); // Trigger the explode animation

        // If the projectile collides with an object tagged as "Enemy", damage it
        if (collision.tag == "Enemy")
            collision.GetComponent<Health>()?.TakeDamage(1); // Call TakeDamage method on Health component
    }

    // Set the direction and activate the projectile
    public void SetDirection(float _direction)
    {
        lifetime = 0; // Reset the lifetime
        direction = _direction; // Set the direction
        gameObject.SetActive(true); // Activate the projectile
        hit = false; // Reset the hit flag
        boxCollider.enabled = true; // Enable the collider

        float localScaleX = transform.localScale.x; // Get the local scale on X-axis
        // Flip the projectile horizontally if the direction changes
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z); // Set the new local scale
    }

    // Deactivate the projectile
    private void Deactivate()
    {
        gameObject.SetActive(false); // Deactivate the projectile
    }
}
