using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Supertext.Base.BackgroundTasks;
using Supertext.Base.Scheduling;

namespace Supertext.Base.Hosting.Extensions
{
    public static class JobSchedulerResumingExtension
    {
        public static IHost ResumeShutdownJobs<TJobResumer>(this IHost host) where TJobResumer : IScheduledsJobResumer
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var backgroundTaskQueue = services.GetRequiredService<IBackgroundTaskQueue>();

                backgroundTaskQueue.QueueBackgroundWorkItem(async (factory, cancellationToken) =>
                                                            {
                                                                var logger = factory.Create<ILogger<TJobResumer>>();
                                                                var jobsResumer = factory.Create<TJobResumer>();
                                                                logger.LogInformation("Start resuming scheduled jobs.");
                                                                await jobsResumer.ResumeAsync(factory, cancellationToken).ConfigureAwait(false);
                                                                logger.LogInformation("Start resuming scheduled jobs completed.");
                                                            });
            }
            return host;
        }
    }
}