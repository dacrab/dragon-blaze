using NUnit.Framework;
using Core.Events;

namespace DragonBlaze.Tests
{
    public class EventBusTests
    {
        [SetUp]
        public void Setup()
        {
            typeof(EventBus)
                .GetMethod("Reset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                ?.Invoke(null, null);
        }

        [Test]
        public void RaiseScoreChanged_InvokesSubscribers()
        {
            int received = -1;
            EventBus.OnScoreChanged += v => received = v;
            EventBus.RaiseScoreChanged(42);
            Assert.AreEqual(42, received);
        }

        [Test]
        public void RaiseHealthChanged_InvokesWithCorrectValues()
        {
            float hp = 0, max = 0;
            EventBus.OnHealthChanged += (c, m) => { hp = c; max = m; };
            EventBus.RaiseHealthChanged(50f, 100f);
            Assert.AreEqual(50f, hp);
            Assert.AreEqual(100f, max);
        }

        [Test]
        public void UnsubscribedHandler_DoesNotReceive()
        {
            int count = 0;
            void Handler(int _) => count++;
            EventBus.OnScoreChanged += Handler;
            EventBus.OnScoreChanged -= Handler;
            EventBus.RaiseScoreChanged(1);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void RaisePlayerDied_InvokesOnce()
        {
            int count = 0;
            EventBus.OnPlayerDied += () => count++;
            EventBus.RaisePlayerDied();
            EventBus.RaisePlayerDied();
            Assert.AreEqual(2, count);
        }
    }
}
