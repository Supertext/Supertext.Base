using System;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Core.Configuration;
using Supertext.Base.Factory;
using Supertext.Base.Modularity;
using Supertext.Base.Specs.Factory.AttributeComponents;
using Supertext.Base.Specs.Factory.CustomKeyComponents;

namespace Supertext.Base.Specs.Factory
{
    [TestClass]
    public class AutofacKeyFactoryTest
    {
        [TestMethod]
        public void Create_WhenKeySubComponentsExists_ThenKeyFactoryCreatesCorrectSubComponent()
        {
            var context = CreateComponentContext();

            var testee = context.Resolve<IComponentDispatcher>();

            var resultOne = testee.DoSomething(ComponentKeyType.One);
            var resultTwo = testee.DoSomething(ComponentKeyType.Two);

            resultOne.Should().BeEquivalentTo(ComponentOneToCreate.ReturnValue);
            resultTwo.Should().BeEquivalentTo(ComponentTwoToCreate.ReturnValue);
        }

        [TestMethod]
        public void Create_WhenOneSubComponentsForTwoKeyExists_ThenKeyFactoryCreatesCorrectSubComponent()
        {
            var context = CreateComponentContext();

            var testee = context.Resolve<IComponentDispatcher>();

            var resultTwo = testee.DoSomething(ComponentKeyType.Three);

            resultTwo.Should().BeEquivalentTo(ComponentTwoToCreate.ReturnValue);
        }

        [TestMethod]
        public void Create_WhenNoRegistrationForSpecificKeyExists_ThenDefaultComponentCreates()
        {
            var context = CreateComponentContext();

            var testee = context.Resolve<IComponentDispatcher>();

            var resultTwo = testee.DoSomething(ComponentKeyType.NotRegistered);

            resultTwo.Should().BeEquivalentTo(DefaultComponentToCreate.ReturnValue);
        }

        [TestMethod]
        public void Create_WhenAttributeSubComponentsExists_ThenKeyFactoryCreatesCorrectSubComponent()
        {
            var context = CreateComponentContext();

            var testee = context.Resolve<IAttributeComponentDispatcher>();

            var resultOne = testee.DoSomething(AttributeOneComponentToCreate.ReturnValue);
            var resultTwo = testee.DoSomething(AttributeTwoComponentToCreate.ReturnValue);

            resultOne.Should().BeEquivalentTo(AttributeOneComponentToCreate.ReturnValue);
            resultTwo.Should().BeEquivalentTo(AttributeTwoComponentToCreate.ReturnValue);
        }

        [TestMethod]
        public void Create_WhenDefaultAttributeSubComponentsExists_ThenKeyFactoryUseDefaultComponent()
        {
            var context = CreateComponentContext();

            var testee = context.Resolve<IAttributeComponentDispatcher>();

            var resultOne = testee.DoSomething("unknown-key");

            resultOne.Should().BeEquivalentTo(DefaultAttributeComponentToCreate.ReturnValue);
        }

        [TestMethod]
        public void ComponentExists_WhenComponentWithNotRegisteredDependency_ThenTrue()
        {
            var context = CreateComponentContext();

            var testee = context.Resolve<IKeyFactory<string, IAttributeComponentToCreate>>();

            var result = testee.ComponentExists("dependencies");

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CreateComponent_WhenComponentWithNotRegisteredDependency_ThenException()
        {
            var context = CreateComponentContext();

            var testee = context.Resolve<IKeyFactory<string, IAttributeComponentToCreate>>();

            testee.Invoking(factory => factory.CreateComponent("dependencies")).Should().Throw<DependencyResolutionException>();
        }
        
        [TestMethod]
        public void CreateComponent_DerivedConfigTypeHasComponentKey_ThenResolved()
        {
            var context = CreateComponentContextWithConfigRegistration();

            var testee = context.Resolve<IKeyFactory<string, ConfigBase>>();

            var result = testee.CreateComponent("test");

            result.Url.Should().Be("localhost");
        }

        private static IComponentContext CreateComponentContext()
        {
            var containerBuilder = CreateContainerBuilderWithDefaultRegistrations();
            return containerBuilder.Build();
        }

        private IComponentContext CreateComponentContextWithConfigRegistration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                                       .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                       .AddJsonFile("appsettings.json");
            var configuration = configurationBuilder.Build();

            var containerBuilder = CreateContainerBuilderWithDefaultRegistrations();
            containerBuilder.RegisterAllConfigurationsInAssembly(configuration, GetType().Assembly);
            return containerBuilder.Build();
        }

        private static ContainerBuilder CreateContainerBuilderWithDefaultRegistrations()
        {
            var containerBuilder = new ContainerBuilder();

            RegisterKeyServices(containerBuilder);
            RegisterAttributedServices(containerBuilder);

            containerBuilder.RegisterGeneric(typeof(AutofacKeyFactory<,>)).As(typeof(IKeyFactory<,>));
            return containerBuilder;
        }

        private static void RegisterAttributedServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AttributeOneComponentToCreate>().As<IAttributeComponentToCreate>();
            containerBuilder.RegisterType<AttributeTwoComponentToCreate>().As<IAttributeComponentToCreate>();
            containerBuilder.RegisterType<DefaultAttributeComponentToCreate>().As<IAttributeComponentToCreate>();
            containerBuilder.RegisterType<AttributeComponentDispatcher>().As<IAttributeComponentDispatcher>();
            containerBuilder.RegisterType<AttributeComponentWithDependencies>().As<IAttributeComponentToCreate>();
        }

        private static void RegisterKeyServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ComponentOneToCreate>().Keyed<IKeyComponentToCreate>(ComponentKeyType.One);
            containerBuilder.RegisterType<ComponentTwoToCreate>().Keyed<IKeyComponentToCreate>(ComponentKeyType.Two);
            containerBuilder.RegisterType<ComponentTwoToCreate>().Keyed<IKeyComponentToCreate>(ComponentKeyType.Three);
            containerBuilder.RegisterType<DefaultComponentToCreate>().KeyedDefault(typeof(IKeyComponentToCreate));
            containerBuilder.RegisterType<ComponentDispatcher>().As<IComponentDispatcher>();
        }
    }
}