using UnityEngine;
using Core.Services;

namespace Gameplay.Items.PowerUps
{
    [CreateAssetMenu(fileName = "InvisibilityPowerUp", menuName = "DragonBlaze/Power Ups/Invisibility")]
    public sealed class InvisibilityPowerUp : PowerUpSO
    {
        public override void Apply(IPlayer player, float duration) => _ = HideAfterDelayAsync(player, duration);

        static async Awaitable HideAfterDelayAsync(IPlayer player, float duration)
        {
            player.SetInvisibility(true);
            await Awaitable.WaitForSecondsAsync(duration);
            player.SetInvisibility(false);
        }
    }
}
