using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Supertext.Base.Extensions;
using Supertext.Base.Factory;
using Supertext.Base.Net.Mail;
using Supertext.Base.Scheduling;
using Timer = System.Threading.Timer;

namespace Supertext.Base.Hosting.Scheduling
{
    /// <summary>
    /// Service class for scheduling long running tasks. Implements <see cref="T:Microsoft.Extensions.Hosting.IHostedService" />.
    /// </summary>
    /// <typeparam name="TJobPayload">Generic type, which is representing the type of the payload, used for the scheduled job.</typeparam>
    public class SchedulerHostedService<TJobPayload> : BackgroundService
    {
        private readonly IHostEnvironment _environment;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger _logger;
        private readonly IDictionary<Guid, JobItem> _scheduledJobs;
        private readonly IJobSchedulingObserver<TJobPayload> _jobSchedulingObserver;

        public SchedulerHostedService(ILoggerFactory loggerFactory,
                                      IHostEnvironment environment,
                                      ILifetimeScope lifetimeScope)
        {
            _environment = environment;
            _lifetimeScope = lifetimeScope;
            _jobSchedulingObserver = _lifetimeScope.Resolve<IJobSchedulingObserver<TJobPayload>>();
            _logger = loggerFactory.CreateLogger<SchedulerHostedService<TJobPayload>>();
            _scheduledJobs = new ConcurrentDictionary<Guid, JobItem>();

            _logger.LogInformation($"{nameof(SchedulerHostedService<TJobPayload>)} started. Environment: {_environment.EnvironmentName}; ApplicationName: {_environment.ApplicationName}");
        }

        public bool IsScheduledJobsQueueEmpty => _scheduledJobs.IsEmpty();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SchedulerHostedService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await _jobSchedulingObserver.DequeueAsync(stoppingToken).ConfigureAwait(false);
                if (job is JobCancellation<TJobPayload>)
                {
                    Cleanup(job.Id);
                    continue;
                }

                var scheduledJob = new Timer(ExecuteJob, job.Id, job.DueTime, new TimeSpan(-1));
                _scheduledJobs.Add(job.Id, new JobItem(job, scheduledJob, new CancellationTokenSource()));
            }

            _logger.LogInformation("SchedulerHostedService is stopping.");
        }

        private void Cleanup(Guid jobId)
        {
            _scheduledJobs[jobId].CancellationTokenSource.Cancel();
            _scheduledJobs[jobId].Timer.Dispose();
            _scheduledJobs.Remove(jobId);
        }

        private void ExecuteJob(object state)
        {
            var jobId = state is Guid guid ? guid : default;
            var jobItem = _scheduledJobs[jobId];
            var job = jobItem.Job;

            try
            {
                using (var scope = _lifetimeScope.BeginLifetimeScope())
                {
                    _logger.LogInformation($"Starting with job with id: {job.Id}. Target: {job.WorkItem.Target}, Method name: {job.WorkItem.Method.Name}");
                    var factory = scope.Resolve<IFactory>();
                    if (jobItem.CancellationTokenSource.IsCancellationRequested)
                    {
                        _logger.LogInformation($"Job has been cancelled. Id: {job.Id}");
                        return;
                    }
                    job.WorkItem(factory, jobItem.Job.Payload, jobItem.CancellationTokenSource.Token).GetAwaiter().GetResult();
                    _logger.LogInformation($"Job finished. Id: {job.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred executing job. Id: {jobId}");
                SendErrorEmail(ex).GetAwaiter().GetResult();
            }
            finally
            {
                Cleanup(jobId);
            }
        }

        private async Task SendErrorEmail(Exception exception)
        {
            try
            {
                var subject = $"[{_environment.EnvironmentName.ToUpperInvariant()}] Error occurred while executing a scheduled job of application {_environment.ApplicationName}";
                var message = $"Exception message: {exception.Message}{Environment.NewLine}{Environment.NewLine}StackTrace: {exception.StackTrace}";
                var from = new PersonInfo($"SchedulerHostedService:{_environment.ApplicationName}", "development@supertext.com");
                var to = new PersonInfo($"Developers", "development@supertext.com");

                var mailInfo = new EmailInfo(subject,
                                             message,
                                             from,
                                             to);
                await using (var scope = _lifetimeScope.BeginLifetimeScope())
                {
                    var mailService = scope.Resolve<IMailService>();
                    await mailService.SendAsync(mailInfo).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Sending an email about failing job failed");
            }
        }

        private class JobItem
        {
            public JobItem(Job<TJobPayload> job, Timer timer, CancellationTokenSource cancellationToken)
            {
                Job = job;
                Timer = timer;
                CancellationTokenSource = cancellationToken;
            }

            public Job<TJobPayload> Job { get; }

            public Timer Timer { get; }

            public CancellationTokenSource CancellationTokenSource { get; }
        }
    }
}