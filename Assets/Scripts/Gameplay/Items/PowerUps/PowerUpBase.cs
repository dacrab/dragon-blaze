using UnityEngine;
using System.Collections;
using Core.Constants;
using Core.Utilities;
using Gameplay.Characters.Player;

public abstract class PowerUpBase : MonoBehaviour
{
    protected float duration = 5f;
    protected Coroutine powerUpCoroutine;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.Tags.Player)) return;
        var playerPowerups = collision.GetComponent<PlayerPowerups>();
        if (playerPowerups == null) return;

        ActivatePowerUp(playerPowerups);
        if (powerUpCoroutine != null) StopCoroutine(powerUpCoroutine);
        powerUpCoroutine = StartCoroutine(PowerUpTimer(playerPowerups));
        StartCoroutine(FadeOutAndInSprite());
        GetComponent<Collider2D>()?.SetEnabled(false);
    }

    protected abstract void ActivatePowerUp(PlayerPowerups playerPowerups);
    protected abstract void DeactivatePowerUp(PlayerPowerups playerPowerups);

    protected void ActivateIndicator(string powerUpName, Sprite powerUpImage) =>
        FindFirstObjectByType<PowerUpIndicatorManager>()?.ActivateIndicator(powerUpName, powerUpImage, duration);

    protected IEnumerator FadeOutAndInSprite()
    {
        for (float i = 1f; i >= 0; i -= Time.deltaTime)
        {
            if (spriteRenderer != null) spriteRenderer.color = spriteRenderer.color.WithAlpha(i);
            yield return null;
        }
        yield return new WaitForSeconds(duration);
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            if (spriteRenderer != null) spriteRenderer.color = spriteRenderer.color.WithAlpha(i);
            yield return null;
        }
    }

    protected IEnumerator PowerUpTimer(PlayerPowerups playerPowerups)
    {
        yield return new WaitForSeconds(duration);
        DeactivatePowerUp(playerPowerups);
        GetComponent<Collider2D>()?.SetEnabled(true);
    }
}
