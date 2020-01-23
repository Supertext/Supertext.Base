﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Abstractions;

namespace Supertext.Base.BackgroundTasks
{
    /// <summary QueuedHostedService="();">
    /// Service for enqueue long running tasks which are performed sequentially in hosted service.
    /// Workings together with QueuedHostedService which needs to be registered in Startup.cs as hosted service as services.AddHostedService()
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<ILifetimeScopeAbstraction, CancellationToken, Task> workItem);

        bool IsQueueEmpty();
    }
}