using Autofac;
using Supertext.Base.BackgroundTasks;
using Supertext.Base.Hosting.Queuing;

namespace Supertext.Base.Hosting
{
    public class HostingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BackgroundTaskQueue>().As<IBackgroundTaskQueue>().SingleInstance();
        }
    }
}