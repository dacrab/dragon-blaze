using UnityEngine;
using System.Collections;
using Core.Constants;

namespace Gameplay.Items.PowerUps
{

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class PowerUpBase : MonoBehaviour
{
    [SerializeField] protected float duration = 5f;

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
        StartCoroutine(PowerUpRoutine(player));
    }

    protected abstract void Activate(Characters.Player.Player player);
    protected abstract void Deactivate(Characters.Player.Player player);

    protected void ShowIndicator(string name, Sprite icon) =>
        PowerUpIndicatorManager.Instance?.ActivateIndicator(name, icon, duration);

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