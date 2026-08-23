using System;
using System.IO;
using NUnit.Framework;
using UnityEngine.TestTools;
using Core.Persistence;

namespace DragonBlaze.Tests
{
    public class SaveServiceTests
    {
        string path;
        SaveService service;

        [SetUp]
        public void Setup()
        {
            path = Path.Combine(Path.GetTempPath(), $"dragonblaze_save_test_{Guid.NewGuid():N}.json");
            service = new SaveService(path);
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(path)) File.Delete(path);
        }

        [Test]
        public void SaveDataExists_IsFalse_WithoutFile() => Assert.IsFalse(service.SaveDataExists());

        [Test]
        public void Load_ReturnsNull_WithoutFile() => Assert.IsNull(service.Load());

        [Test]
        public void Save_ThenLoad_RoundTrips()
        {
            service.Save(new SaveData { totalCoins = 42, levelName = "Level3" });

            Assert.IsTrue(service.SaveDataExists());
            var data = service.Load();
            Assert.AreEqual(42, data.totalCoins);
            Assert.AreEqual("Level3", data.levelName);
            Assert.AreEqual(SaveService.CurrentVersion, data.version);
        }

        [Test]
        public void Load_MigratesLegacySaves_ToCurrentVersion()
        {
            File.WriteAllText(path, "{\"totalCoins\":7,\"levelName\":\"Level1\"}");
            var data = service.Load();
            Assert.IsNotNull(data);
            Assert.AreEqual(7, data.totalCoins);
            Assert.AreEqual("Level1", data.levelName);
            Assert.AreEqual(SaveService.CurrentVersion, data.version);
        }

        [Test]
        public void Load_CorruptFile_ReturnsNull()
        {
            LogAssert.ignoreFailingMessages = true;
            File.WriteAllText(path, "not json {{{");
            Assert.IsNull(service.Load());
            LogAssert.ignoreFailingMessages = false;
        }
    }
}