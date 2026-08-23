using Core.Services;

namespace Gameplay.Items.PowerUps
{
    [CreateAssetMenu(fileName = "InvisibilityPowerUp", menuName = "DragonBlaze/Power Ups/Invisibility")]
    public sealed class InvisibilityPowerUp : PowerUpSO
    {
        public override void Apply(IPlayer player, float duration) => player.SetInvisibilityFor(duration);
    }
}
