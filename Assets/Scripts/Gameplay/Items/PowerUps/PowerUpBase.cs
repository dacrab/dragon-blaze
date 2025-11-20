using UnityEngine;
using System.Collections;
using Core.Constants;
using Gameplay.Characters.Player;

public abstract class PowerUpBase : MonoBehaviour
{
    #region Fields
    protected float duration = 5f; // Default duration, can be overridden in derived classes
    protected Coroutine powerUpCoroutine;
    protected SpriteRenderer spriteRenderer;
    #endregion

    #region Unity Lifecycle Methods
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.Tags.Player)) return;

        PlayerPowerups playerPowerups = collision.GetComponent<PlayerPowerups>();
        // Fallback to PlayerMovement shim if PlayerPowerups is not directly found (though it should be)
        if (playerPowerups == null)
        {
             PlayerMovement pm = collision.GetComponent<PlayerMovement>();
             if (pm != null) playerPowerups = pm.GetComponent<PlayerPowerups>();
        }

        if (playerPowerups != null)
        {
            ActivatePowerUp(playerPowerups);
            if (powerUpCoroutine != null)
                StopCoroutine(powerUpCoroutine);
            powerUpCoroutine = StartCoroutine(PowerUpTimer(playerPowerups));
            StartCoroutine(FadeOutAndInSprite());

            GetComponent<Collider2D>().enabled = false;
        }
    }
    #endregion

    #region Power-Up Methods
    protected abstract void ActivatePowerUp(PlayerPowerups playerPowerups);
    protected abstract void DeactivatePowerUp(PlayerPowerups playerPowerups);

    protected void ActivateIndicator(string powerUpName, Sprite powerUpImage)
    {
        PowerUpIndicatorManager indicatorManager = FindObjectOfType<PowerUpIndicatorManager>();
        if (indicatorManager != null)
        {
            indicatorManager.ActivateIndicator(powerUpName, powerUpImage, duration);
        }
        // Else suppress warning if manager is optional
    }
    #endregion

    #region Coroutines
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

    protected IEnumerator PowerUpTimer(PlayerPowerups playerPowerups)
    {
        yield return new WaitForSeconds(duration);
        DeactivatePowerUp(playerPowerups);
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = true;
    }
    #endregion
}
