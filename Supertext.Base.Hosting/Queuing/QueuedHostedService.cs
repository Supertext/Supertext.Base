using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Supertext.Base.Factory;
using Supertext.Base.Net.Mail;

namespace Supertext.Base.Hosting.Queuing
{
    /// <summary>
    /// Service class for implementing a long running <see cref="T:Microsoft.Extensions.Hosting.IHostedService" />.
    /// </summary>
    public class QueuedHostedService : BackgroundService
    {
        private readonly IHostEnvironment _environment;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger _logger;

        public QueuedHostedService(IBackgroundTaskQueueObserver taskQueue,
                                   ILoggerFactory loggerFactory,
                                   IHostEnvironment environment,
                                   ILifetimeScope lifetimeScope)
        {
            _environment = environment;
            _lifetimeScope = lifetimeScope;
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();

            _logger.LogInformation($"{nameof(QueuedHostedService)} started. Environment: {_environment.EnvironmentName}; ApplicationName: {_environment.ApplicationName}");
        }

        private IBackgroundTaskQueueObserver TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken).ConfigureAwait(false);

                try
                {
                    using (var scope = _lifetimeScope.BeginLifetimeScope())
                    {
                        _logger.LogInformation($"Starting with workitem. Target: {workItem.Target}, Method name: {workItem.Method.Name}");
                        var factory = scope.Resolve<IFactory>();
                        await workItem(factory, stoppingToken).ConfigureAwait(false);
                        TaskQueue.WorkItemFinished();
                        _logger.LogInformation($"Workitem finished. Target: {workItem.Target}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred executing {nameof(workItem)}.");
                    await SendErrorEmail(ex).ConfigureAwait(false);
                    TaskQueue.WorkItemFinished();
                }
            }

            _logger.LogInformation("Queued Hosted Service is stopping.");
        }

        private async Task SendErrorEmail(Exception exception)
        {
            try
            {
                var subject = $"[{_environment.EnvironmentName.ToUpperInvariant()}] Error occurred while executing queued workitem of application {_environment.ApplicationName}";
                var message = $"Exception message: {exception.Message}{Environment.NewLine}{Environment.NewLine}StackTrace: {exception.StackTrace}";
                var from = new PersonInfo($"QueuedHostedService:{_environment.ApplicationName}", "development@supertext.com");
                var to = new PersonInfo($"Developers", "development@supertext.com");

                var mailInfo = new EmailInfo(subject,
                                             message,
                                             from,
                                             to);
                using (var scope = _lifetimeScope.BeginLifetimeScope())
                {
                    var mailService = scope.Resolve<IMailService>();
                    await mailService.SendAsync(mailInfo).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Sending an email about failing workitem failed");
            }
        }
    }
}