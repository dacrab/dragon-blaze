using NUnit.Framework;
using Core.Services;
using Gameplay.Characters.Player;

namespace DragonBlaze.Tests
{
    public class PlayerStatsTests
    {
        [Test]
        public void Factor_IsOne_WithoutModifiers() =>
            Assert.AreEqual(1f, new PlayerStats().Factor(PlayerStat.Speed), 0.0001f);

        [Test]
        public void AddModifier_ScalesFactor()
        {
            var stats = new PlayerStats();
            stats.Add(PlayerStat.Speed, 2f, 5f);
            Assert.AreEqual(2f, stats.Factor(PlayerStat.Speed), 0.0001f);
        }

        [Test]
        public void MultipleModifiers_Multiply()
        {
            var stats = new PlayerStats();
            stats.Add(PlayerStat.Jump, 2f, 5f);
            stats.Add(PlayerStat.Jump, 1.5f, 5f);
            Assert.AreEqual(3f, stats.Factor(PlayerStat.Jump), 0.0001f);
        }

        [Test]
        public void Modifiers_AreIndependentPerStat()
        {
            var stats = new PlayerStats();
            stats.Add(PlayerStat.Speed, 2f, 5f);
            Assert.AreEqual(1f, stats.Factor(PlayerStat.Jump), 0.0001f);
        }

        [Test]
        public void Tick_PartialTime_KeepsModifier()
        {
            var stats = new PlayerStats();
            stats.Add(PlayerStat.Damage, 2f, 10f);
            stats.Tick(2f);
            Assert.AreEqual(2f, stats.Factor(PlayerStat.Damage), 0.0001f);
        }

        [Test]
        public void Tick_ExpiresModifier()
        {
            var stats = new PlayerStats();
            stats.Add(PlayerStat.Damage, 2f, 1f);
            stats.Tick(1.5f);
            Assert.AreEqual(1f, stats.Factor(PlayerStat.Damage), 0.0001f);
        }

        [Test]
        public void AddModifier_IgnoresNonPositiveFactor()
        {
            var stats = new PlayerStats();
            stats.Add(PlayerStat.Damage, 0f, 5f);
            stats.Add(PlayerStat.Damage, -1f, 5f);
            Assert.AreEqual(1f, stats.Factor(PlayerStat.Damage), 0.0001f);
        }

        [Test]
        public void Tick_NoModifiers_DoesNotThrow() => Assert.DoesNotThrow(() => new PlayerStats().Tick(1f));
    }
}
