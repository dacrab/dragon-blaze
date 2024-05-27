using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth; // Starting health value
    public float currentHealth { get; private set; } // Current health value
    private Animator anim; // Animator component reference
    private bool dead; // Flag indicating if the player is dead

    [Header("Invulnerability Frames")]
    [SerializeField] private float iFramesDuration; // Duration of invulnerability frames
    [SerializeField] private int numberOfFlashes; // Number of flashes during invulnerability
    private SpriteRenderer spriteRend; // SpriteRenderer component reference

    [Header("Components")]
    [SerializeField] private Behaviour[] components; // List of components to deactivate upon death
    private bool invulnerable; // Flag indicating if the player is invulnerable

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound; // Sound clip played upon death
    [SerializeField] private AudioClip hurtSound; // Sound clip played when taking damage

    [Header("Particle Systems")]
    [SerializeField] private GameObject hitParticleSystemPrefab; // Particle system prefab for player hit effects
    [SerializeField] private GameObject deathParticleSystemPrefab; // Particle system prefab for player death effects

    private PlayerMovement playerMovement;

    private void Awake()
    {
        currentHealth = startingHealth; // Initialize current health to starting health
        anim = GetComponent<Animator>(); // Get reference to the Animator component
        spriteRend = GetComponent<SpriteRenderer>(); // Get reference to the SpriteRenderer component
        playerMovement = GetComponent<PlayerMovement>();  // Get the PlayerMovement component
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable || (playerMovement != null && playerMovement.IsInvisible())) return; // Check if invulnerable or invisible

        // Decrease current health by the damage amount within the range of 0 to startingHealth
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // If the player is still alive, trigger the hurt animation and start invulnerability frames
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
            SoundManager.instance.PlaySound(hurtSound); // Play the hurt sound

            // Play particle system at player's position for hit effects
            if (hitParticleSystemPrefab != null)
            {
                Instantiate(hitParticleSystemPrefab, transform.position, Quaternion.identity);
            }
        }
        else
        {
            if (!dead)
            {
                // If the player is dead, deactivate all attached component classes, trigger death animation, and play death sound
                foreach (Behaviour component in components)
                {
                    component.enabled = false;
                }

                anim.SetBool("grounded", true);
                anim.SetTrigger("die");

                dead = true; // Set the dead flag to true
                SoundManager.instance.PlaySound(deathSound); // Play the death sound

                // Play particle system at player's position for death effects
                if (deathParticleSystemPrefab != null)
                {
                    Instantiate(deathParticleSystemPrefab, transform.position, Quaternion.identity);
                }
            }
        }
    }

    public void AddHealth(float _value)
    {
        // Add health within the range of 0 to startingHealth
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true; // Set the invulnerable flag to true
        Physics2D.IgnoreLayerCollision(10, 11, true); // Ignore collision between player and enemies temporarily

        // Flash the player sprite for a certain number of times during invulnerability frames
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); // Set the sprite color to red with reduced alpha
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2)); // Wait for half of the invulnerability duration
            spriteRend.color = Color.white; // Set the sprite color back to white
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2)); // Wait for the remaining half of the invulnerability duration
        }

        Physics2D.IgnoreLayerCollision(10, 11, false); // Re-enable collision between player and enemies
        invulnerable = false; // Set the invulnerable flag to false
    }

    private void Deactivate()
    {
        gameObject.SetActive(false); // Deactivate the game object
    }

    public void Respawn()
    {
        // Reset health, animation triggers, and flags
        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("Idle");
        StartCoroutine(Invulnerability());
        dead = false;

        // Activate all attached component classes
        foreach (Behaviour component in components)
        {
            component.enabled = true;
        }

        // Enable the BoxCollider2D component to allow interactions again
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
