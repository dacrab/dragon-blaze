using System;
using System.IO;
using NUnit.Framework;
using UnityEngine.TestTools;
using Core.Persistence;

namespace DragonBlaze.Tests
{
    public class SaveServiceTests
    {
        string path = "";
        SaveService service = null!;

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
            Assert.IsNotNull(data);
            Assert.AreEqual(42, data!.totalCoins);
            Assert.AreEqual("Level3", data.levelName);
        }

        [Test]
        public void Load_CorruptFile_ReturnsNull()
        {
            LogAssert.ignoreFailingMessages = true;
            File.WriteAllText(path, "not json {{{");
            Assert.IsNull(service.Load());
            LogAssert.ignoreFailingMessages = false;
        }

        [Test]
        public void Save_LeavesNoTempFileBehind()
        {
            service.Save(new SaveData { totalCoins = 1, levelName = "Level1" });
            Assert.IsFalse(File.Exists(path + ".tmp"), "temp file should be moved into place, not left behind");
        }

        [Test]
        public void Save_OverwritesExistingSave()
        {
            service.Save(new SaveData { totalCoins = 5, levelName = "Level1" });
            service.Save(new SaveData { totalCoins = 9, levelName = "Level2" });

            var data = service.Load();
            Assert.AreEqual(9, data!.totalCoins);
            Assert.AreEqual("Level2", data.levelName);
        }
    }
}