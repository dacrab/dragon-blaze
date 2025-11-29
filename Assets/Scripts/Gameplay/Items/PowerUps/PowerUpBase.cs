using UnityEngine;
using System.Collections;
using Core.Constants;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public abstract class PowerUpBase : MonoBehaviour
    {
        [Header("Power-Up Settings")]
        [SerializeField] protected float duration = 5f;

        protected Coroutine powerUpCoroutine;
        protected SpriteRenderer spriteRenderer;
        protected Collider2D col;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;

            var playerPowerups = collision.GetComponent<PlayerPowerups>();
            if (playerPowerups == null) return;

            ActivatePowerUp(playerPowerups);
            
            if (powerUpCoroutine != null)
                StopCoroutine(powerUpCoroutine);
            
            powerUpCoroutine = StartCoroutine(PowerUpTimer(playerPowerups));
            StartCoroutine(FadeOutAndInSprite());
            
            col.enabled = false;
        }

        protected abstract void ActivatePowerUp(PlayerPowerups playerPowerups);
        
        protected virtual void DeactivatePowerUp(PlayerPowerups playerPowerups) { }

        protected void ActivateIndicator(string powerUpName, Sprite powerUpImage)
        {
            var indicatorManager = FindFirstObjectByType<PowerUpIndicatorManager>();
            indicatorManager?.ActivateIndicator(powerUpName, powerUpImage, duration);
        }

        protected IEnumerator FadeOutAndInSprite()
        {
            float fadeSpeed = 2f;
            
            for (float t = 1f; t >= 0; t -= Time.deltaTime * fadeSpeed)
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = new Color(1, 1, 1, t);
                yield return null;
            }

            yield return new WaitForSeconds(duration);

            for (float t = 0; t <= 1; t += Time.deltaTime * fadeSpeed)
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = new Color(1, 1, 1, t);
                yield return null;
            }
        }

        protected IEnumerator PowerUpTimer(PlayerPowerups playerPowerups)
        {
            yield return new WaitForSeconds(duration);
            DeactivatePowerUp(playerPowerups);
            if (col != null) col.enabled = true;
        }
    }
}
