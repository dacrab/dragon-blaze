using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private float healthValue;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupParticles;
    #endregion

    #region Unity Lifecycle Methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HandlePickup(collision);
        }
    }
    #endregion

    #region Private Methods
    private void HandlePickup(Collider2D playerCollider)
    {
        PlayPickupSound();
        AddHealthToPlayer(playerCollider);
        PlayParticleEffect();
        DeactivateCollectible();
    }

    private void PlayPickupSound()
    {
        if (SoundManager.instance != null && pickupSound != null)
        {
            SoundManager.instance.PlaySound(pickupSound);
        }
    }

    private void AddHealthToPlayer(Collider2D playerCollider)
    {
        Health playerHealth = playerCollider.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.AddHealth(healthValue);
        }
    }

    private void PlayParticleEffect()
    {
        if (pickupParticles != null)
        {
            pickupParticles.transform.position = transform.position;
            pickupParticles.SetActive(true);
            ParticleSystem particles = pickupParticles.GetComponent<ParticleSystem>();
            if (particles != null)
            {
                particles.Play();
            }
            else
            {
                Debug.LogWarning("The pickupParticles GameObject does not have a ParticleSystem component.");
            }
        }
    }

    private void DeactivateCollectible()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
