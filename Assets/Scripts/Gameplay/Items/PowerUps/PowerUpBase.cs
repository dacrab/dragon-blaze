using UnityEngine;
using System.Collections;
using Core.Constants;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    public abstract class PowerUpBase : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected float duration = 5f;
        
        [Header("Visual")]
        [SerializeField] protected float fadeOutDuration = 0.5f;
        [SerializeField] protected float fadeInDuration = 0.5f;

        protected Coroutine powerUpCoroutine;
        protected SpriteRenderer spriteRenderer;
        protected Collider2D powerUpCollider;
        protected PowerUpIndicatorManager indicatorManager;
        protected bool isActive;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            powerUpCollider = GetComponent<Collider2D>();
            indicatorManager = FindFirstObjectByType<PowerUpIndicatorManager>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            var playerPowerups = collision.GetComponent<PlayerPowerups>();
            if (playerPowerups == null || (isActive)) return;

            ActivatePowerUp(playerPowerups);
            if (powerUpCoroutine != null) StopCoroutine(powerUpCoroutine);
            powerUpCoroutine = StartCoroutine(PowerUpTimer(playerPowerups));
            StartCoroutine(FadeSprite());
            if (powerUpCollider != null) powerUpCollider.enabled = false;
        }

        protected abstract void ActivatePowerUp(PlayerPowerups playerPowerups);
        protected abstract void DeactivatePowerUp(PlayerPowerups playerPowerups);

        protected void ActivateIndicator(string name, Sprite image) => indicatorManager?.ActivateIndicator(name, image, duration);

        protected IEnumerator FadeSprite()
        {
            if (spriteRenderer == null) yield break;
            Color c = spriteRenderer.color;
            
            float t = 0;
            while (t < fadeOutDuration)
            {
                t += Time.deltaTime;
                spriteRenderer.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t / fadeOutDuration));
                yield return null;
            }
            
            yield return new WaitForSeconds(duration - fadeOutDuration - fadeInDuration);
            
            t = 0;
            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                spriteRenderer.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0f, 1f, t / fadeInDuration));
                yield return null;
            }
        }

        protected IEnumerator PowerUpTimer(PlayerPowerups playerPowerups)
        {
            isActive = true;
            yield return new WaitForSeconds(duration);
            DeactivatePowerUp(playerPowerups);
            isActive = false;
            if (powerUpCollider != null) powerUpCollider.enabled = true;
        }
    }
}
