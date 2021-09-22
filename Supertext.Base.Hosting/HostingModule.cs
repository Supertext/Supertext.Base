using Autofac;
using Supertext.Base.BackgroundTasks;
using Supertext.Base.Hosting.Queuing;
using Supertext.Base.Hosting.Scheduling;
using Supertext.Base.Scheduling;

namespace Supertext.Base.Hosting
{
    public class HostingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BackgroundTaskQueue>()
                   .As<IBackgroundTaskQueueObserver>()
                   .As<IBackgroundTaskQueue>()
                   .SingleInstance();

            builder.RegisterGeneric(typeof(JobSchedulingQueue<>))
                   .As(typeof(IJobSchedulingObserver<>))
                   .As(typeof(IJobScheduler<>))
                   .SingleInstance();
        }
    }
}