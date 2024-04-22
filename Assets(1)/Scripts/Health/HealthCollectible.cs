using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue; // Amount of health the collectible provides
    [SerializeField] private AudioClip pickupSound; // Sound played when the collectible is picked up

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only).
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") // If the collider has the "Player" tag
        {
            SoundManager.instance.PlaySound(pickupSound); // Play pickup sound
            collision.GetComponent<Health>().AddHealth(healthValue); // Add health to the player
            gameObject.SetActive(false); // Deactivate the collectible object
        }
    }
}
