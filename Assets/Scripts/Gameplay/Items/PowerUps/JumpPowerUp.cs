using UnityEngine;
using Core.Services;

namespace Gameplay.Items.PowerUps
{
    [CreateAssetMenu(fileName = "JumpPowerUp", menuName = "DragonBlaze/Power Ups/Jump")]
    public sealed class JumpPowerUp : PowerUpSO
    {
        public float multiplier = 2f;

        public override void Apply(IPlayer player, float duration) =>
            player.AddModifier(PlayerStat.Jump, multiplier, duration);
    }
}
