using UnityEngine;
using System.Collections;

public abstract class PowerUpBase : MonoBehaviour
{
    protected float duration = 5f; // Default duration, can be overridden in derived classes
    protected Coroutine powerUpCoroutine;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected IEnumerator FadeOutAndInSprite()
    {
        // Fade out
        for (float i = 1f; i >= 0; i -= Time.deltaTime)
        {
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = i;
                spriteRenderer.color = c;
            }
            yield return null;
        }

        // Wait for the duration of the powerup
        yield return new WaitForSeconds(duration);

        // Fade in
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = i;
                spriteRenderer.color = c;
            }
            yield return null;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            ActivatePowerUp(playerMovement);
            if (powerUpCoroutine != null)
                StopCoroutine(powerUpCoroutine);
            powerUpCoroutine = StartCoroutine(PowerUpTimer(playerMovement));
            StartCoroutine(FadeOutAndInSprite());

            GetComponent<Collider2D>().enabled = false;
        }
    }

    protected IEnumerator PowerUpTimer(PlayerMovement playerMovement)
    {
        yield return new WaitForSeconds(duration);
        DeactivatePowerUp(playerMovement);
        GetComponent<Collider2D>().enabled = true;
    }

    protected abstract void ActivatePowerUp(PlayerMovement playerMovement);
    protected abstract void DeactivatePowerUp(PlayerMovement playerMovement);

    protected void ActivateIndicator(string powerUpName, Sprite powerUpImage)
    {
        PowerUpIndicatorManager indicatorManager = FindObjectOfType<PowerUpIndicatorManager>();
        if (indicatorManager != null)
        {
            indicatorManager.ActivateIndicator(powerUpName, powerUpImage, duration);
        }
        else
        {
            Debug.LogWarning("PowerUpIndicatorManager not found in the scene.");
        }
    }
}
