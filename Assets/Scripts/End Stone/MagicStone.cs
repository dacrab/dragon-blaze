using UnityEngine;
using UnityEngine.UI;

public class MagicStone : MonoBehaviour
{
    public SpriteRenderer interactIndicator; // Assign the UI indicator in the Inspector
    private bool isPlayerInRange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactIndicator.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactIndicator.SetActive(false);
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        // Call the function to play particle system and transition to loading screen
        StartCoroutine(InteractSequence());
    }

    private IEnumerator InteractSequence()
    {
        // Play the particle system
        // Assuming you have a ParticleSystem component attached to the magic stone
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        // Wait for the particle system to finish
        yield return new WaitForSeconds(ps.main.duration);

        // Transition to the loading screen
        UIManager.Instance.Play(); // Make sure you have a singleton pattern in UIManager
    }
}
