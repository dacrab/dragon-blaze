using UnityEngine;
using Core.Constants;

namespace Environment.Traps
{

public abstract class TrapBase : MonoBehaviour
{
    [SerializeField] protected float damage = 10f;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.Tags.Player)) return;
        if (collision.GetComponent<Gameplay.Characters.Player.Player>() is { IsInvisible: true }) return;
        collision.GetComponent<Gameplay.Health.Health>()?.TakeDamage(damage);
    }
}
}