using Core.Services;

namespace Gameplay.Items.PowerUps
{
    [CreateAssetMenu(fileName = "JumpPowerUp", menuName = "DragonBlaze/Power Ups/Jump")]
    public sealed class JumpPowerUp : MultiplierPowerUp
    {
        protected override PlayerStat Stat => PlayerStat.Jump;
    }
}
