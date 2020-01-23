using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Factory;
using Supertext.Base.Modules;

namespace Supertext.Base.Specs.Factory
{
    [TestClass]
    public class AutofacFactoryTest
    {
        private IFactory _testee;

        [TestInitialize]
        public void Init()
        {
            var container = CreateComponentContext();
            _testee = container.Resolve<IFactory>();
        }

        [TestMethod]
        public void Create_ComponentIsRegistered_Created()
        {
            var component = _testee.Create<IAnyComponent>();

            component.Should().BeOfType<AnyComponent>();
        }

        private static IComponentContext CreateComponentContext()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<AnyComponent>().As<IAnyComponent>();
            containerBuilder.RegisterModule<BaseModule>();
            return containerBuilder.Build();
        }

        private interface IAnyComponent
        {
        }

        private class AnyComponent : IAnyComponent
        {
        }
    }
}