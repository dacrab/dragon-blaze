using Core.Services;

namespace Gameplay.Items.PowerUps
{
    /// <summary>Shared timed-multiplier effect; subclasses pick the stat.</summary>
    public abstract class MultiplierPowerUp : PowerUpSO
    {
        public float multiplier = 2f;

        protected abstract PlayerStat Stat { get; }

        public override void Apply(IPlayer player, float duration) =>
            player.AddModifier(Stat, multiplier, duration);
    }
}
