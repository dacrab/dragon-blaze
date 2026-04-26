using UnityEngine;
using System.Collections;
using Core.Constants;
using UI.HUD;

namespace Gameplay.Items.PowerUps
{
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public abstract class PowerUpBase : MonoBehaviour
    {
        [SerializeField] protected float duration = 5f;
        [SerializeField] protected Sprite icon;
        [SerializeField] protected string powerUpName;

        protected SpriteRenderer sprite;
        protected Collider2D col;

        protected virtual void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            if (collision.GetComponent<Characters.Player.Player>() is not { } player) return;

            Activate(player);
            PowerUpIndicatorManager.Instance?.ActivateIndicator(powerUpName, icon, duration);
            StartCoroutine(PowerUpRoutine(player));
        }

        protected abstract void Activate(Characters.Player.Player player);
        protected abstract void Deactivate(Characters.Player.Player player);

        IEnumerator PowerUpRoutine(Characters.Player.Player player)
        {
            col.enabled = false;
            sprite.enabled = false;
            yield return new WaitForSeconds(duration);
            Deactivate(player);
            col.enabled = true;
            sprite.enabled = true;
        }
    }
}
}