using NUnit.Framework;
using UnityEngine;
using Core.Constants;
using Core.State;

namespace DragonBlaze.Tests
{
    public class GameConfigTests
    {
        [Test]
        public void DefaultStateSettings_Gameplay_RunsTimeLocked()
        {
            var settings = GameConfig.DefaultStateSettings(GameState.Gameplay);
            Assert.AreEqual(1f, settings.timeScale, 0.0001f);
            Assert.IsFalse(settings.cursorVisible);
            Assert.IsTrue(settings.cursorLocked);
        }

        [Test]
        public void DefaultStateSettings_MainMenu_ShowsCursor()
        {
            var settings = GameConfig.DefaultStateSettings(GameState.MainMenu);
            Assert.AreEqual(1f, settings.timeScale, 0.0001f);
            Assert.IsTrue(settings.cursorVisible);
            Assert.IsFalse(settings.cursorLocked);
        }

        [Test]
        public void DefaultStateSettings_PausedAndGameOver_StopTime()
        {
            Assert.AreEqual(0f, GameConfig.DefaultStateSettings(GameState.Paused).timeScale, 0.0001f);
            Assert.AreEqual(0f, GameConfig.DefaultStateSettings(GameState.GameOver).timeScale, 0.0001f);
        }

        [Test]
        public void GetStateSettings_FallsBackToDefault_WhenMissing()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.stateSettings = null;
            Assert.AreEqual(1f, config.GetStateSettings(GameState.Gameplay).timeScale, 0.0001f);
        }

        [Test]
        public void GetStateSettings_UsesConfiguredOverride()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.stateSettings = new[]
            {
                new StateSettings { state = GameState.Gameplay, timeScale = 0.5f, cursorVisible = true, cursorLocked = false }
            };
            var settings = config.GetStateSettings(GameState.Gameplay);
            Assert.AreEqual(0.5f, settings.timeScale, 0.0001f);
            Assert.IsTrue(settings.cursorVisible);
            Assert.IsFalse(settings.cursorLocked);
        }
    }
}
