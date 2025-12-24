using UnityEngine;
using System.Collections;
using Core.Constants;
using Core.Services;
using Core.Utilities;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    /// <summary>
    /// Base class for all power-ups. Handles activation, duration, and visual feedback.
    /// </summary>
    public abstract class PowerUpBase : MonoBehaviour
    {
        [Header("Power-Up Settings")]
        [SerializeField] protected float duration = 5f;
        [SerializeField] protected bool canStack = false;
        
        [Header("Visual Feedback")]
        [SerializeField] protected float fadeOutDuration = 0.5f;
        [SerializeField] protected float fadeInDuration = 0.5f;

        protected Coroutine powerUpCoroutine;
        protected Coroutine fadeCoroutine;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] protected Collider2D powerUpCollider;
        [AutoWire(AutoWireAttribute.WireType.Service, required: false)]
        [SerializeField] protected PowerUpIndicatorManager indicatorManager;
        protected bool isActive;

        protected virtual void Awake()
        {
            AutoWireHelper.WireAllFields(this);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            
            var playerPowerups = collision.GetComponent<PlayerPowerups>();
            if (playerPowerups == null) return;

            if (isActive && !canStack) return;

            ActivatePowerUp(playerPowerups);
            
            // Stop existing coroutines safely
            StopPowerUpCoroutines();
            
            powerUpCoroutine = StartCoroutine(PowerUpTimer(playerPowerups));
            fadeCoroutine = StartCoroutine(FadeOutAndInSprite());
            
            DisableCollider();
        }

        protected abstract void ActivatePowerUp(PlayerPowerups playerPowerups);
        protected abstract void DeactivatePowerUp(PlayerPowerups playerPowerups);

        protected void ActivateIndicator(string powerUpName, Sprite powerUpImage)
        {
            indicatorManager?.ActivateIndicator(powerUpName, powerUpImage, duration);
        }

        protected void StopPowerUpCoroutines()
        {
            this.SafeStopCoroutine(ref powerUpCoroutine);
            this.SafeStopCoroutine(ref fadeCoroutine);
        }

        protected void DisableCollider()
        {
            if (powerUpCollider != null)
                powerUpCollider.enabled = false;
        }

        protected void EnableCollider()
        {
            if (powerUpCollider != null)
                powerUpCollider.enabled = true;
        }

        protected IEnumerator FadeOutAndInSprite()
        {
            if (spriteRenderer == null) yield break;

            // Fade out
            float elapsed = 0f;
            Color startColor = spriteRenderer.color;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                spriteRenderer.color = startColor.WithAlpha(alpha);
                yield return null;
            }
            spriteRenderer.color = startColor.WithAlpha(0f);

            // Wait for power-up duration minus fade times
            float waitTime = duration - fadeOutDuration - fadeInDuration;
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);

            // Fade in
            elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                spriteRenderer.color = startColor.WithAlpha(alpha);
                yield return null;
            }
            spriteRenderer.color = startColor.WithAlpha(1f);
        }

        protected IEnumerator PowerUpTimer(PlayerPowerups playerPowerups)
        {
            isActive = true;
            yield return new WaitForSeconds(duration);
            
            DeactivatePowerUp(playerPowerups);
            isActive = false;
            EnableCollider();
            
            powerUpCoroutine = null;
        }

        protected virtual void OnDisable()
        {
            StopPowerUpCoroutines();
        }
    }
}
