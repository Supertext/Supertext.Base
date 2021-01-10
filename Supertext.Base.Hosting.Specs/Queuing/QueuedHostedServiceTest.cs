using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.BackgroundTasks;
using Supertext.Base.Hosting.Queuing;
using Supertext.Base.Modules;
using Supertext.Base.Net;
using Supertext.Base.Net.Mail;

namespace Supertext.Base.Hosting.Specs.Queuing
{
    [TestClass]
    public class QueuedHostedServiceTest
    {
        private QueuedHostedService _testee;
        private IComponentContext _container;
        private static IMailService _mailService;

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
        public async Task ScheduleJob_BackgroundTaskIsDequeued_BackgroundWorkCanExecute()
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

        [TestMethod]
        public async Task ScheduleJob_WorkitemThrowsException_EmailIsSent()
        {
            EmailInfo sentEmailInfo = null;
            A.CallTo(() => _mailService.SendAsync(A<EmailInfo>._)).Invokes(invocation => sentEmailInfo = invocation.GetArgument<EmailInfo>(0));
            var backgroundTaskQueue = _container.Resolve<IBackgroundTaskQueue>();
            backgroundTaskQueue.QueueBackgroundWorkItem((factory, token) => throw new ApplicationException("Error!!"));

            while (!backgroundTaskQueue.IsQueueEmpty())
            {
                await Task.Delay(200);
            }

            sentEmailInfo.Should().NotBeNull();
            sentEmailInfo.Subject.Should().Be("[DEVELOPMENT] Error occurred while executing queued workitem of application TestApplication");
        }

        private static IComponentContext CreateComponentContext()
        {
            var containerBuilder = new ContainerBuilder();
            _mailService = A.Fake<IMailService>();
            var hostEnvironment = A.Fake<IHostEnvironment>();
            A.CallTo(() => hostEnvironment.EnvironmentName).Returns("Development");
            A.CallTo(() => hostEnvironment.ApplicationName).Returns("TestApplication");
            containerBuilder.RegisterType<QueuedHostedService>();
            containerBuilder.RegisterType<AnyComponent>().As<IAnyComponent>();
            containerBuilder.RegisterModule<BaseModule>();
            containerBuilder.RegisterModule<HostingModule>();
            containerBuilder.RegisterModule<NetModule>();
            containerBuilder.RegisterInstance(_mailService);
            containerBuilder.RegisterInstance(hostEnvironment);
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