using UnityEngine;
using Core.Constants;

namespace Gameplay.Combat
{
    /// <summary>Damages the player once on contact (spikes, hazards). Continuous hazards use <see cref="ContactDamagePerSecond"/>.</summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class ContactDamage : MonoBehaviour
    {
        [SerializeField] float damage = 1f;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) collision.DamagePlayer(damage);
        }
    }
}
