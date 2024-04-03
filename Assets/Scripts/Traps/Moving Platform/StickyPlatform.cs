using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    // Called when another collider enters this trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.gameObject.name == "Player")
        {
            // Set the parent of the player object to this platform
            collision.gameObject.transform.SetParent(transform);
        }  
    }

    // Called when another collider exits this trigger collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.gameObject.name == "Player")
        {
            // Set the parent of the player object to null (no parent)
            collision.gameObject.transform.SetParent(null);
        }
    }
}
