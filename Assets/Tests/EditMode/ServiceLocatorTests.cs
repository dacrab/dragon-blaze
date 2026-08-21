using NUnit.Framework;
using Core.Services;

namespace DragonBlaze.Tests
{
    public class ServiceLocatorTests
    {
        class Marker { }

        [SetUp]
        public void Setup() => ServiceLocator.Unregister<Marker>();

        [TearDown]
        public void Teardown() => ServiceLocator.Unregister<Marker>();

        [Test]
        public void Get_ReturnsNull_WhenNotRegistered() => Assert.IsNull(ServiceLocator.Get<Marker>());

        [Test]
        public void Get_ReturnsRegisteredInstance()
        {
            var service = new Marker();
            ServiceLocator.Register(service);
            Assert.AreSame(service, ServiceLocator.Get<Marker>());
        }

        [Test]
        public void Register_OverwritesPrevious()
        {
            var first = new Marker();
            var second = new Marker();
            ServiceLocator.Register(first);
            ServiceLocator.Register(second);
            Assert.AreSame(second, ServiceLocator.Get<Marker>());
        }

        [Test]
        public void Unregister_RemovesService()
        {
            ServiceLocator.Register(new Marker());
            ServiceLocator.Unregister<Marker>();
            Assert.IsNull(ServiceLocator.Get<Marker>());
        }
    }
}
