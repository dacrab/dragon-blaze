using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject lootPrefab; // The prefab for the loot (e.g., coins, power-ups)
    public Transform spawnPoint; // The point where loot spawns
    public float spawnForce = 5f; // Force to apply to the spawned loot

    private bool isOpen = false; // Track whether the chest is open

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        isOpen = true;
        // Play chest opening animation or sound (if you have one)

        // Spawn loot dynamically
        SpawnLoot();
    }

    private void SpawnLoot()
    {
        if (lootPrefab != null && spawnPoint != null)
        {
            GameObject lootInstance = Instantiate(lootPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody2D lootRb = lootInstance.GetComponent<Rigidbody2D>();

            // Apply force to the spawned loot (optional)
            Vector2 spawnDirection = Vector2.up; // Customize this direction
            lootRb.AddForce(spawnDirection * spawnForce, ForceMode2D.Impulse);
        }
    }
}
