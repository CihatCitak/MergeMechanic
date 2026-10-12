using Game.Core.Services;
using NUnit.Framework;

namespace Game.Tests
{
    public class ServiceLocatorTests
    {
        private interface IFooService { }
        private interface IBarService { }
        private class FooBarService : IFooService, IBarService { }

        private FooBarService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new FooBarService();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Unbind<IFooService>(_service);
            ServiceLocator.Unbind<IBarService>(_service);
        }

        [Test]
        public void Bind_SameObjectUnderMultipleInterfaces_ResolvesEach()
        {
            ServiceLocator.Bind<IFooService>(_service);
            ServiceLocator.Bind<IBarService>(_service);

            Assert.AreSame(_service, ServiceLocator.Get<IFooService>());
            Assert.AreSame(_service, ServiceLocator.Get<IBarService>());
        }

        [Test]
        public void Bind_Twice_KeepsFirst()
        {
            ServiceLocator.Bind<IFooService>(_service);

            LogUtility.WithoutLogs(() => ServiceLocator.Bind<IFooService>(new FooBarService()));

            Assert.AreSame(_service, ServiceLocator.Get<IFooService>());
        }

        [Test]
        public void Get_Unbound_ReturnsNull()
        {
            IFooService service = null;

            LogUtility.WithoutLogs(() => service = ServiceLocator.Get<IFooService>());

            Assert.IsNull(service);
        }

        [Test]
        public void Unbind_RemovesService()
        {
            ServiceLocator.Bind<IFooService>(_service);
            IFooService service = _service;

            ServiceLocator.Unbind<IFooService>(_service);
            LogUtility.WithoutLogs(() => service = ServiceLocator.Get<IFooService>());

            Assert.IsNull(service);
        }

        [Test]
        public void Unbind_WithDifferentObject_KeepsBoundService()
        {
            ServiceLocator.Bind<IFooService>(_service);

            ServiceLocator.Unbind<IFooService>(new FooBarService());

            Assert.AreSame(_service, ServiceLocator.Get<IFooService>());
        }
    }
}
