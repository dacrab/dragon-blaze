using UnityEngine;
using Core.Services;

namespace Gameplay.Items.PowerUps
{
    /// <summary>
    /// ScriptableObject describing a power-up effect. Duration is managed by the effect itself
    /// (timed stat modifiers expire automatically, invisibility schedules its own revert).
    /// </summary>
    public abstract class PowerUpSO : ScriptableObject
    {
        public abstract void Apply(IPlayer player, float duration);
    }
}
