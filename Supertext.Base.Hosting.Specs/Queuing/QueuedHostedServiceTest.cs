using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Supertext.Base.BackgroundTasks;
using Supertext.Base.Hosting.Queuing;
using Supertext.Base.Modules;
using Supertext.Base.Net;
using Supertext.Base.Net.Mail;
using Supertext.Base.Tracing;

namespace Supertext.Base.Hosting.Specs.Queuing
{
    [TestClass]
    public class QueuedHostedServiceTest
    {
        private QueuedHostedService? _testee;
        private IComponentContext? _container;
        private static IMailService? _mailService;
        private Guid _correlationId;

        [TestInitialize]
        public void Init()
        {
            Log.Logger = new LoggerConfiguration()
                         .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                         .Enrich.FromLogContext()
                         .Enrich.WithCorrelationIdHeader()
                         .CreateLogger();

            _container = CreateComponentContext();
            var loggerFactory = _container.Resolve<ILoggerFactory>();
            loggerFactory.AddSerilog();
            _testee = _container.Resolve<QueuedHostedService>();
            _testee.StartAsync(CancellationToken.None);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _testee!.StopAsync(CancellationToken.None);
        }

        [TestMethod]
        public async Task QueueBackgroundWorkItem_BackgroundTaskIsDequeued_BackgroundWorkCanExecute()
        {
            var correlationId = Guid.NewGuid();
            var backgroundTaskQueue = _container!.Resolve<IBackgroundTaskQueue>();
            IAnyComponent component = null!;
            backgroundTaskQueue.QueueBackgroundWorkItem(async (factory, _) =>
                                                        {
                                                            component = factory.Create<IAnyComponent>();
                                                            var tracingProvider = factory.Create<ITracingProvider>();
                                                            _correlationId = tracingProvider.CorrelationId;
                                                            await Task.CompletedTask;
                                                        },
                                                        correlationId);

            while (!backgroundTaskQueue.IsQueueEmpty())
            {
                await Task.Delay(200);
            }

            component.Should().BeOfType<AnyComponent>();
            _correlationId.Should().Be(correlationId);
        }

        [TestMethod]
        public async Task QueueBackgroundWorkItem_WorkitemThrowsException_EmailIsSent()
        {
            EmailInfo sentEmailInfo = null!;
            A.CallTo(() => _mailService!.SendAsync(A<EmailInfo>._, CancellationToken.None)).Invokes(invocation => sentEmailInfo = invocation.GetArgument<EmailInfo>(0)!);
            var backgroundTaskQueue = _container!.Resolve<IBackgroundTaskQueue>();
            backgroundTaskQueue.QueueBackgroundWorkItem((_, _) => throw new ApplicationException("Error!!"));

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
            var services = new ServiceCollection();
            services.AddLogging();
            containerBuilder.Populate(services);

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