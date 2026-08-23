using Core.Services;

namespace Gameplay.Items.PowerUps
{
    [CreateAssetMenu(fileName = "SpeedPowerUp", menuName = "DragonBlaze/Power Ups/Speed")]
    public sealed class SpeedPowerUp : MultiplierPowerUp
    {
        protected override PlayerStat Stat => PlayerStat.Speed;
    }
}
