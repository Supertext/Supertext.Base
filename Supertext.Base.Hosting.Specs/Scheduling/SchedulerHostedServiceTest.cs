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
using Supertext.Base.Hosting.Scheduling;
using Supertext.Base.Modules;
using Supertext.Base.Net;
using Supertext.Base.Net.Mail;
using Supertext.Base.Scheduling;

namespace Supertext.Base.Hosting.Specs.Scheduling
{
    [TestClass]
    public class SchedulerHostedServiceTest
    {
        private SchedulerHostedService<Guid> _testee;
        private IComponentContext _container;
        private static IMailService _mailService;
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
            _testee = _container.Resolve<SchedulerHostedService<Guid>>();
            _testee.StartAsync(CancellationToken.None);

            _correlationId = Guid.NewGuid();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _testee.StopAsync(CancellationToken.None);
        }

        [TestMethod]
        public async Task ScheduleJob_JobIsScheduled_JobIsExecutedAccordingly()
        {
            var jobScheduler = _container.Resolve<IJobScheduler<Guid>>();
            Guid payload = default;

            var job = new Job<Guid>(Guid.NewGuid(),
                                    TimeSpan.FromMilliseconds(50),
                                    Guid.NewGuid(),
                                    async (factory, guid, cancellationToken) =>
                                    {
                                        payload = guid;
                                        await Task.CompletedTask;
                                    },
                                    _correlationId);

            jobScheduler.ScheduleJob(job);

            do
            {
                await Task.Delay(20);
            } while (!_testee.IsScheduledJobsQueueEmpty);

            payload.Should().Be(job.Payload);
        }

        [TestMethod]
        public async Task ScheduleJob_TwoJobsAreScheduled_JobsAreExecutedAccordingly()
        {
            var jobScheduler = _container.Resolve<IJobScheduler<Guid>>();
            Guid payload1 = default;
            Guid payload2 = default;

            var job1 = new Job<Guid>(Guid.NewGuid(),
                                    TimeSpan.FromMilliseconds(50),
                                    Guid.NewGuid(),
                                    async (factory, guid, cancellationToken) =>
                                    {
                                        payload1 = guid;
                                        await Task.CompletedTask;
                                    },
                                    _correlationId);
            var job2 = new Job<Guid>(Guid.NewGuid(),
                                    TimeSpan.FromMilliseconds(40),
                                    Guid.NewGuid(),
                                    async (factory, guid, cancellationToken) =>
                                    {
                                        payload2 = guid;
                                        await Task.CompletedTask;
                                    },
                                     Guid.NewGuid());

            jobScheduler.ScheduleJob(job1);
            jobScheduler.ScheduleJob(job2);

            do
            {
                await Task.Delay(20);
            } while (!_testee.IsScheduledJobsQueueEmpty);

            payload1.Should().Be(job1.Payload);
            payload2.Should().Be(job2.Payload);
        }

        [TestMethod]
        public async Task ScheduleJob_WorkitemThrowsException_EmailIsSent()
        {
            EmailInfo sentEmailInfo = null;
            A.CallTo(() => _mailService.SendAsync(A<EmailInfo>._)).Invokes(invocation => sentEmailInfo = invocation.GetArgument<EmailInfo>(0));
            var jobScheduler = _container.Resolve<IJobScheduler<Guid>>();

            var job = new Job<Guid>(Guid.NewGuid(),
                                    TimeSpan.FromMilliseconds(50),
                                    Guid.NewGuid(),
                                    (factory, guid, cancellationToken) =>
                                    {
                                        throw new ApplicationException("Error!!");
                                    },
                                    _correlationId);

            jobScheduler.ScheduleJob(job);

            do
            {
                await Task.Delay(20);
            } while (!_testee.IsScheduledJobsQueueEmpty);

            sentEmailInfo.Should().NotBeNull();
            sentEmailInfo.Subject.Should().Be("[DEVELOPMENT] Error occurred while executing a scheduled job of application TestApplication");
        }

        [TestMethod]
        public async Task CancelJob_TwoJobsAreScheduledButOneHasBeenCancelled_OnlyOneJobIsExecutedAccordingly()
        {
            var jobScheduler = _container.Resolve<IJobScheduler<Guid>>();
            Guid payload1 = default;
            Guid payload2 = default;

            var job1 = new Job<Guid>(Guid.NewGuid(),
                                     TimeSpan.FromMilliseconds(300),
                                     Guid.NewGuid(),
                                     async (factory, guid, cancellationToken) =>
                                     {
                                         payload1 = guid;
                                         await Task.CompletedTask;
                                     },
                                     _correlationId);
            var job2 = new Job<Guid>(Guid.NewGuid(),
                                     TimeSpan.FromMilliseconds(200),
                                     Guid.NewGuid(),
                                     async (factory, guid, cancellationToken) =>
                                     {
                                         payload2 = guid;
                                         await Task.CompletedTask;
                                     },
                                     Guid.NewGuid());

            jobScheduler.ScheduleJob(job1);
            jobScheduler.ScheduleJob(job2);

            jobScheduler.CancelJob(job1.Id);

            do
            {
                await Task.Delay(20);
            } while (!_testee.IsScheduledJobsQueueEmpty);

            payload1.Should().Be(default(Guid));
            payload2.Should().Be(job2.Payload);
        }

        private static IComponentContext CreateComponentContext()
        {
            var containerBuilder = new ContainerBuilder();
            _mailService = A.Fake<IMailService>();
            var hostEnvironment = A.Fake<IHostEnvironment>();
            A.CallTo(() => hostEnvironment.EnvironmentName).Returns("Development");
            A.CallTo(() => hostEnvironment.ApplicationName).Returns("TestApplication");
            containerBuilder.RegisterType<SchedulerHostedService<Guid>>();
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