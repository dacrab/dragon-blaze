using UnityEngine;
using System.Collections;

public abstract class PowerUpBase : MonoBehaviour
{
    protected float duration = 5f; // Default duration, can be overridden in derived classes
    protected Coroutine powerUpCoroutine;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            ActivatePowerUp(playerMovement);
            if (powerUpCoroutine != null)
                StopCoroutine(powerUpCoroutine);
            powerUpCoroutine = StartCoroutine(PowerUpTimer(playerMovement));

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
