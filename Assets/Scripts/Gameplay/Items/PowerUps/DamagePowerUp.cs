using UnityEngine;
using Core.Services;

namespace Gameplay.Items.PowerUps
{
    [CreateAssetMenu(fileName = "DamagePowerUp", menuName = "DragonBlaze/Power Ups/Damage")]
    public sealed class DamagePowerUp : PowerUpSO
    {
        public float multiplier = 2f;

        public override void Apply(IPlayer player, float duration) =>
            player.AddModifier(PlayerStat.Damage, multiplier, duration);
    }
}
