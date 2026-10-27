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

        [Test]
        public void IsMenuScene_MatchesConfiguredMenus()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.menuScenes = new[] { "MainMenu", "Credits" };
            Assert.IsTrue(config.IsMenuScene("MainMenu"));
            Assert.IsTrue(config.IsMenuScene("Credits"));
            Assert.IsFalse(config.IsMenuScene("Level1"));
            Assert.IsFalse(config.IsMenuScene(null));
            Assert.IsFalse(config.IsMenuScene(""));
        }

        [Test]
        public void IsMenuScene_HandlesMissingArray()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.menuScenes = null;
            Assert.IsFalse(config.IsMenuScene("MainMenu"));
        }

        [Test]
        public void TryGetNextLevel_WalksLevelOrder()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.levelOrder = new[] { "MainMenu", "Level1", "Level2" };

            Assert.IsTrue(config.TryGetNextLevel("Level1", out var next));
            Assert.AreEqual("Level2", next);
        }

        [Test]
        public void TryGetNextLevel_ReturnsFalseAtEnd()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.levelOrder = new[] { "MainMenu", "Level1" };

            Assert.IsFalse(config.TryGetNextLevel("Level1", out var next));
            Assert.IsNull(next);
        }

        [Test]
        public void TryGetNextLevel_ReturnsFalseForUnknownScene()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.levelOrder = new[] { "MainMenu", "Level1" };

            Assert.IsFalse(config.TryGetNextLevel("Nowhere", out _));
        }

        [Test]
        public void FirstLevelAndMenuNames_ComeFromLevelOrder()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.levelOrder = new[] { "Menu", "Start" };
            Assert.AreEqual("Menu", config.MainMenuSceneName);
            Assert.AreEqual("Start", config.FirstLevelSceneName);
        }

        [Test]
        public void FirstLevelSceneName_IsNull_WhenNoLevelsConfigured()
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.levelOrder = new[] { "Menu" };
            Assert.IsNull(config.FirstLevelSceneName);
        }
    }
}
