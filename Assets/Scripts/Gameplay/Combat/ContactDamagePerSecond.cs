using UnityEngine;
using Core.Constants;

namespace Gameplay.Combat
{
    /// <summary>Continuous hazard damage, expressed per second so framerate and physics step rate do not scale it.</summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class ContactDamagePerSecond : MonoBehaviour
    {
        [SerializeField] float damagePerSecond = 1f;

        void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player))
                collision.DamagePlayer(damagePerSecond * Time.deltaTime);
        }
    }
}
