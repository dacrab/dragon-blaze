using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private int damage; // Damage inflicted by the arrow
    [SerializeField] private float attackCooldown; // Cooldown between attacks
    [SerializeField] private Transform firePoint; // Point from which arrows are fired
    [SerializeField] private GameObject[] arrows; // Array of arrow GameObjects
    private float cooldownTimer; // Timer to track the cooldown period

    [Header("SFX")]
    [SerializeField] private AudioClip arrowSound; // Sound clip for arrow firing

    private void Attack()
    {
        cooldownTimer = 0; // Reset the cooldown timer

        SoundManager.instance.PlaySound(arrowSound); // Play the arrow firing sound
        arrows[FindArrow()].transform.position = firePoint.position; // Set the position of the arrow
        arrows[FindArrow()].GetComponent<EnemyProjectile>().ActivateProjectile(); // Activate the arrow projectile
    }

    private int FindArrow()
    {
        // Find an inactive arrow in the array to reuse
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i].activeInHierarchy)
                return i;
        }
        return 0; // Return the index of the first arrow if all are active
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime; // Update the cooldown timer

        // If cooldown time has elapsed and player is visible, attack
        if (cooldownTimer >= attackCooldown && PlayerIsVisible())
        {
            Attack();
        }
    }

    private bool PlayerIsVisible()
    {
        // Check if the player is visible to the trap
        PlayerMovement playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        return playerMovement != null && playerMovement.IsVisible();
    }    
}
