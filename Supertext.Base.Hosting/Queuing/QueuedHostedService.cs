using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Supertext.Base.Abstractions;

namespace Supertext.Base.Hosting.Queuing
{
    /// <summary>
    /// Service class for implementing a long running <see cref="T:Microsoft.Extensions.Hosting.IHostedService" />.
    /// </summary>
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger _logger;

        public QueuedHostedService(IBackgroundTaskQueueObserver taskQueue, ILoggerFactory loggerFactory, ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
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
                        var scopeAbstraction = new LifetimeScopeAbstraction(scope);
                        await workItem(scopeAbstraction, stoppingToken).ConfigureAwait(false);
                        TaskQueue.WorkItemFinished();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred executing {nameof(workItem)}.");
                    TaskQueue.WorkItemFinished();
                }
            }

            _logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}