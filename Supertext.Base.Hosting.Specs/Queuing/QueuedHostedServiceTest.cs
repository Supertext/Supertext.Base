using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.BackgroundTasks;
using Supertext.Base.Hosting.Queuing;
using Supertext.Base.Modules;

namespace Supertext.Base.Hosting.Specs.Queuing
{
    [TestClass]
    public class QueuedHostedServiceTest
    {
        private QueuedHostedService _testee;
        private IComponentContext _container;

        [TestInitialize]
        public void Init()
        {
            _container = CreateComponentContext();
            _testee = _container.Resolve<QueuedHostedService>();
            _testee.StartAsync(CancellationToken.None);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _testee.StopAsync(CancellationToken.None);
        }

        [TestMethod]
        public async Task StartAsync_BackgroundTaskIsDequeued_BackgroundWorkCanExecute()
        {
            var backgroundTaskQueue = _container.Resolve<IBackgroundTaskQueue>();
            IAnyComponent component = null;
            backgroundTaskQueue.QueueBackgroundWorkItem(async (factory, token) =>
                                                        {
                                                            component = factory.Create<IAnyComponent>();
                                                            await Task.CompletedTask;
                                                        });

            while (!backgroundTaskQueue.IsQueueEmpty())
            {
                await Task.Delay(200);
            }

            component.Should().BeOfType<AnyComponent>();
        }

        private static IComponentContext CreateComponentContext()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<QueuedHostedService>();
            containerBuilder.RegisterType<AnyComponent>().As<IAnyComponent>();
            containerBuilder.RegisterModule<BaseModule>();
            containerBuilder.RegisterModule<HostingModule>();
            var logger = A.Fake<ILoggerFactory>();
            containerBuilder.RegisterInstance(logger).As<ILoggerFactory>();
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