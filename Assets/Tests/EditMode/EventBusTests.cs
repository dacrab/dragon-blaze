using NUnit.Framework;
using Core.Events;

namespace DragonBlaze.Tests
{
    public class EventBusTests
    {
        [SetUp]
        public void Setup() => EventBus.Clear();

        [Test]
        public void Raise_InvokesSubscribers()
        {
            int received = -1;
            EventBus.Subscribe<ScoreChangedEvent>(e => received = e.Score);
            EventBus.Raise(new ScoreChangedEvent(42));
            Assert.AreEqual(42, received);
        }

        [Test]
        public void Raise_InvokesWithCorrectValues()
        {
            float hp = 0, max = 0;
            EventBus.Subscribe<HealthChangedEvent>(e => { hp = e.Current; max = e.Max; });
            EventBus.Raise(new HealthChangedEvent(50f, 100f));
            Assert.AreEqual(50f, hp);
            Assert.AreEqual(100f, max);
        }

        [Test]
        public void UnsubscribedHandler_DoesNotReceive()
        {
            int count = 0;
            void Handler(ScoreChangedEvent _) => count++;
            EventBus.Subscribe<ScoreChangedEvent>(Handler);
            EventBus.Unsubscribe<ScoreChangedEvent>(Handler);
            EventBus.Raise(new ScoreChangedEvent(1));
            Assert.AreEqual(0, count);
        }

        [Test]
        public void RaiseEmptyPayload_InvokesOncePerRaise()
        {
            int count = 0;
            EventBus.Subscribe<PlayerDiedEvent>(_ => count++);
            EventBus.Raise(new PlayerDiedEvent());
            EventBus.Raise(new PlayerDiedEvent());
            Assert.AreEqual(2, count);
        }
    }
}
