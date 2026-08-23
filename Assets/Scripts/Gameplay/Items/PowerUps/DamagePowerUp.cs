using Core.Services;

namespace Gameplay.Items.PowerUps
{
    [CreateAssetMenu(fileName = "DamagePowerUp", menuName = "DragonBlaze/Power Ups/Damage")]
    public sealed class DamagePowerUp : MultiplierPowerUp
    {
        protected override PlayerStat Stat => PlayerStat.Damage;
    }
}
