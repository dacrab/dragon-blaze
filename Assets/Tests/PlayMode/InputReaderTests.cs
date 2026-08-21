using NUnit.Framework;
using UnityEngine;
using Core.Input;

namespace DragonBlaze.Tests
{
    public class InputReaderTests
    {
        [Test]
        public void Instance_ResolvesFromResources()
        {
            var reader = InputReader.Instance;
            Assert.IsNotNull(reader, "Resources/InputReader.asset is missing or failed to import.");
            Assert.IsTrue(reader.HasActionMaps, "InputReader loaded but its InputActions asset did not resolve.");
        }

        [Test]
        public void EnableGameplayInput_DoesNotThrow()
        {
            var reader = InputReader.Instance;
            if (reader == null) Assert.Ignore("InputReader asset missing.");
            Assert.DoesNotThrow(reader.EnableGameplayInput);
            Assert.DoesNotThrow(reader.EnableUIInput);
        }
    }
}
