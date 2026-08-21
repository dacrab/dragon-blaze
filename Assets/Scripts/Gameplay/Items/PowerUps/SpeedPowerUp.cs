using UnityEngine;
using Core.Services;

namespace Gameplay.Items.PowerUps
{
    [CreateAssetMenu(fileName = "SpeedPowerUp", menuName = "DragonBlaze/Power Ups/Speed")]
    public sealed class SpeedPowerUp : PowerUpSO
    {
        public float multiplier = 2f;

        public override void Apply(IPlayer player, float duration) =>
            player.AddModifier(PlayerStat.Speed, multiplier, duration);
    }
}
